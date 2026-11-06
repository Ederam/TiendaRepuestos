namespace TiendaRepuestos.Application.Services;

using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Ports;

/// <summary>
/// Orquestador del caso de uso de ventas: validación, generación atómica de folios,
/// verificación de stock y costos, y persistencia transaccional.
/// </summary>
public class VentaService
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IConsecutivoService _consecutivoService;
    private readonly IValidator<CrearVentaDto> _crearVentaValidator;
    private readonly ILogger<VentaService> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VentaService"/> inyectando puertos y servicios requeridos.
    /// </summary>
    public VentaService(
        IVentaRepository ventaRepository,
        IProductoRepository productoRepository,
        IConsecutivoService consecutivoService,
        IValidator<CrearVentaDto> crearVentaValidator,
        ILogger<VentaService> logger)
    {
        _ventaRepository = ventaRepository ?? throw new ArgumentNullException(nameof(ventaRepository));
        _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
        _consecutivoService = consecutivoService ?? throw new ArgumentNullException(nameof(consecutivoService));
        _crearVentaValidator = crearVentaValidator ?? throw new ArgumentNullException(nameof(crearVentaValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Consulta el listado consolidado de todas las ventas emitidas.
    /// </summary>
    public async Task<IEnumerable<VentaResponseDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Venta> ventas = await _ventaRepository.GetAllAsync(cancellationToken);
        return ventas.Select(VentaResponseDto.FromEntity);
    }

    /// <summary>
    /// Obtiene una venta específica por su identificador GUID.
    /// </summary>
    public async Task<VentaResponseDto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Venta? venta = await _ventaRepository.GetByIdAsync(id, cancellationToken);
        return venta is null ? null : VentaResponseDto.FromEntity(venta);
    }

    /// <summary>
    /// Procesa una venta en caja registradora respetando el precio acordado en mostrador.
    /// </summary>
    public async Task<VentaResponseDto> CrearVentaAsync(CrearVentaDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Validar reglas de entrada
        await _crearVentaValidator.ValidateAndThrowAsync(dto, cancellationToken);

        // 2. Generación atómica del folio consecutivo oficial
        string consecutivo = await _consecutivoService.ObtenerSiguienteConsecutivoVentaAsync(cancellationToken);
        _logger.LogInformation("Asignando folio consecutivo {NumeroComprobante} a la nueva transacción.", consecutivo);

        // 3. Crear cabecera con snapshot inmutable del comprador
        var nuevaVenta = new Venta(
            consecutivo,
            dto.MetodoPago,
            dto.ClienteId,
            dto.ClienteDocumento,
            dto.ClienteNombre);

        // 4. Validar stock, costos y aplicar precio acordado
        foreach (CrearDetalleVentaDto item in dto.Detalles)
        {
            Producto? producto = await _productoRepository.GetByIdAsync(item.ProductoId, cancellationToken);

            if (producto == null)
            {
                _logger.LogWarning("Transacción abortada. Repuesto con ID {ProductoId} no localizado.", item.ProductoId);
                throw new KeyNotFoundException($"No se encontró el repuesto con ID: {item.ProductoId}");
            }

            if (producto.StockActual < item.Cantidad)
            {
                _logger.LogWarning("Stock insuficiente para '{NombreProducto}'. Disponible: {StockActual}, Solicitado: {Cantidad}",
                    producto.Nombre, producto.StockActual, item.Cantidad);

                throw new InvalidOperationException(
                    $"Stock insuficiente para el repuesto '{producto.Nombre}'. Disponible: {producto.StockActual}, Solicitado: {item.Cantidad}");
            }

            // Resolución del precio: si viene del mostrador (> 0) se toma, de lo contrario el precio de catálogo
            decimal precioAplicado = (item.PrecioUnitario.HasValue && item.PrecioUnitario.Value > 0)
                ? item.PrecioUnitario.Value
                : producto.PrecioVenta;

            // Validación estricta anti-pérdida: No permitir venta por debajo del costo unitario registrado
            if (producto.Costo > 0 && precioAplicado < producto.Costo)
            {
                throw new InvalidOperationException(
                    $"El precio pactado (${precioAplicado:N0}) no puede ser menor al costo (${producto.Costo:N0}) para '{producto.Nombre}'.");
            }

            // Descontar inventario
            producto.AjustarStock(-item.Cantidad);
            await _productoRepository.UpdateAsync(producto, cancellationToken);

            // Agregar renglón con el precio efectivamente aplicado
            nuevaVenta.AgregarDetalle(producto.Id, producto.Nombre, item.Cantidad, precioAplicado);
        }

        // 5. Persistir transacción
        await _ventaRepository.AddAsync(nuevaVenta, cancellationToken);

        _logger.LogInformation("Venta {NumeroComprobante} liquidada con Total: {TotalVenta}",
            nuevaVenta.NumeroComprobante, nuevaVenta.Total);

        return VentaResponseDto.FromEntity(nuevaVenta);
    }
}