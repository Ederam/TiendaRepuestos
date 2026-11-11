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
using TiendaRepuestos.Domain.Models;
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
    private readonly IUsuarioRepository? _usuarioRepository;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VentaService"/> inyectando puertos y servicios requeridos.
    /// </summary>
    public VentaService(
        IVentaRepository ventaRepository,
        IProductoRepository productoRepository,
        IConsecutivoService consecutivoService,
        IValidator<CrearVentaDto> crearVentaValidator,
        ILogger<VentaService> logger,
        IUsuarioRepository? usuarioRepository = null)
    {
        _ventaRepository = ventaRepository ?? throw new ArgumentNullException(nameof(ventaRepository));
        _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
        _consecutivoService = consecutivoService ?? throw new ArgumentNullException(nameof(consecutivoService));
        _crearVentaValidator = crearVentaValidator ?? throw new ArgumentNullException(nameof(crearVentaValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _usuarioRepository = usuarioRepository;
    }

    /// <summary>
    /// Consulta el historial de ventas aplicando aislamiento por rol (RBAC).
    /// Vendedor: siempre filtra por su propio UsuarioId (se ignora <paramref name="vendedorId"/>).
    /// Administrador: si <paramref name="vendedorId"/> viene informado filtra por ese vendedor; si es null retorna el consolidado.
    /// </summary>
    /// <param name="usuarioActualId">Usuario autenticado extraído del token.</param>
    /// <param name="esAdministrador">Indica si el usuario autenticado tiene rol Administrador.</param>
    /// <param name="vendedorId">Filtro opcional de vendedor, solo aplicable para Administradores.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    public async Task<IEnumerable<VentaResponseDto>> ObtenerTodasAsync(
        Guid usuarioActualId,
        bool esAdministrador,
        Guid? vendedorId = null,
        CancellationToken cancellationToken = default)
    {
        Guid? filtroUsuario = esAdministrador ? vendedorId : usuarioActualId;

        IReadOnlyList<VentaConVendedor> ventas = await _ventaRepository.ObtenerHistorialAsync(filtroUsuario, cancellationToken);
        return ventas.Select(VentaResponseDto.FromModel);
    }

    /// <summary>
    /// Obtiene una venta específica por su identificador GUID. Un vendedor solo accede a sus propias ventas.
    /// </summary>
    public async Task<VentaResponseDto?> ObtenerPorIdAsync(
        Guid id,
        Guid usuarioActualId,
        bool esAdministrador,
        CancellationToken cancellationToken = default)
    {
        VentaConVendedor? modelo = await _ventaRepository.ObtenerConVendedorPorIdAsync(id, cancellationToken);
        if (modelo is null || (!esAdministrador && modelo.Venta.UsuarioId != usuarioActualId))
        {
            return null;
        }

        return VentaResponseDto.FromModel(modelo);
    }

    /// <summary>
    /// Procesa una venta en caja registradora respetando el precio acordado en mostrador.
    /// </summary>
    public async Task<VentaResponseDto> CrearVentaAsync(CrearVentaDto dto, Guid? usuarioId = null, CancellationToken cancellationToken = default)
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
            dto.ClienteNombre,
            usuarioId);

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

        string? nombreVendedor = null;
        if (usuarioId.HasValue && usuarioId.Value != Guid.Empty && _usuarioRepository is not null)
        {
            Usuario? usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId.Value, cancellationToken);
            nombreVendedor = usuario?.NombreCompleto;
        }

        _logger.LogInformation("Venta {NumeroComprobante} liquidada por UsuarioId {UsuarioId} ({NombreVendedor}) con Total: {TotalVenta}",
            nuevaVenta.NumeroComprobante, nuevaVenta.UsuarioId, nombreVendedor, nuevaVenta.Total);

        VentaResponseDto respuesta = VentaResponseDto.FromEntity(nuevaVenta);
        respuesta.NombreVendedor = nombreVendedor;
        return respuesta;
    }
}