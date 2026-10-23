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
/// verificación y descuento de existencias de repuestos, y persistencia transaccional.
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
    /// <param name="ventaRepository">Puerto de almacenamiento y consulta de comprobantes.</param>
    /// <param name="productoRepository">Puerto de persistencia de inventario.</param>
    /// <param name="consecutivoService">Puerto generador de secuencias atómicas de comprobantes.</param>
    /// <param name="crearVentaValidator">Validador de reglas para la creación de ventas.</param>
    /// <param name="logger">Servicio de logs estructurados.</param>
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
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Colección de comprobantes mapeados a DTO.</returns>
    public async Task<IEnumerable<VentaResponseDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Venta> ventas = await _ventaRepository.GetAllAsync(cancellationToken);
        return ventas.Select(VentaResponseDto.FromEntity);
    }

    /// <summary>
    /// Obtiene una venta específica por su identificador GUID.
    /// </summary>
    /// <param name="id">Identificador único de la venta.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>DTO de la venta localizada o null en caso contrario.</returns>
    public async Task<VentaResponseDto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Venta? venta = await _ventaRepository.GetByIdAsync(id, cancellationToken);
        return venta is null ? null : VentaResponseDto.FromEntity(venta);
    }

    /// <summary>
    /// Procesa una venta en caja registradora de forma atómica: genera consecutivo, valida stock,
    /// deduce existencias y persiste el comprobante fiscal.
    /// </summary>
    /// <param name="dto">Datos de entrada provistos por el cajero o POS.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>DTO del comprobante generado con subtotales e identificador.</returns>
    /// <exception cref="ValidationException">Si el payload no satisface las reglas de negocio.</exception>
    /// <exception cref="KeyNotFoundException">Si alguno de los repuestos referenciados no existe.</exception>
    /// <exception cref="InvalidOperationException">Si no hay inventario suficiente para suplir la venta.</exception>
    public async Task<VentaResponseDto> CrearVentaAsync(CrearVentaDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Validar reglas de entrada con FluentValidation
        await _crearVentaValidator.ValidateAndThrowAsync(dto, cancellationToken);

        // 2. Generación atómica y segura del consecutivo (ej: VTA-00001)
        string consecutivo = await _consecutivoService.ObtenerSiguienteConsecutivoVentaAsync(cancellationToken);
        _logger.LogInformation("Asignando folio consecutivo {NumeroComprobante} a la nueva transacción.", consecutivo);

        // 3. Crear entidad con snapshot del cliente
        var nuevaVenta = new Venta(
            consecutivo,
            dto.MetodoPago,
            dto.ClienteId,
            dto.ClienteDocumento,
            dto.ClienteNombre);

        // 4. Validar existencia y stock de cada repuesto de forma transaccional
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

            // Descontar inventario mediante método de dominio
            producto.AjustarStock(-item.Cantidad);
            await _productoRepository.UpdateAsync(producto, cancellationToken);

            // Agregar renglón a la factura con el precio oficial vigente en catálogo
            nuevaVenta.AgregarDetalle(producto.Id, producto.Nombre, item.Cantidad, producto.PrecioVenta);
        }

        // 5. Persistir agregado completo
        await _ventaRepository.AddAsync(nuevaVenta, cancellationToken);

        _logger.LogInformation("Venta {NumeroComprobante} confirmada exitosamente. Total: {TotalVenta}",
            nuevaVenta.NumeroComprobante, nuevaVenta.Total);

        return VentaResponseDto.FromEntity(nuevaVenta);
    }
}