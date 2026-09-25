namespace TiendaRepuestos.Application.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Ports;

/// <summary>
/// Servicio del caso de uso encargado de orquestar la creación, consulta y afectación de inventario para ventas.
/// </summary>
public class VentaService
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IProductoRepository _productoRepository;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VentaService"/>.
    /// </summary>
    /// <param name="ventaRepository">Puerto para el almacenamiento de ventas.</param>
    /// <param name="productoRepository">Puerto para el almacenamiento y actualización de repuestos.</param>
    public VentaService(IVentaRepository ventaRepository, IProductoRepository productoRepository)
    {
        _ventaRepository = ventaRepository ?? throw new ArgumentNullException(nameof(ventaRepository));
        _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
    }

    /// <summary>
    /// Obtiene el historial general de ventas procesadas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la tarea.</param>
    /// <returns>Colección de DTOs de ventas.</returns>
    public async Task<IEnumerable<VentaResponseDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Venta> ventas = await _ventaRepository.GetAllAsync(cancellationToken);
        return ventas.Select(VentaResponseDto.FromEntity);
    }

    /// <summary>
    /// Consulta una venta por su identificador único.
    /// </summary>
    /// <param name="id">Identificador único (GUID) de la venta.</param>
    /// <param name="cancellationToken">Token de cancelación de la tarea.</param>
    /// <returns>DTO de la venta si se encuentra; de lo contrario, null.</returns>
    public async Task<VentaResponseDto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Venta? venta = await _ventaRepository.GetByIdAsync(id, cancellationToken);
        return venta is null ? null : VentaResponseDto.FromEntity(venta);
    }

    /// <summary>
    /// Procesa una transacción de venta, descuenta el stock de los repuestos involucrados y persiste el comprobante.
    /// </summary>
    /// <param name="dto">Datos del comprobante e ítems a vender.</param>
    /// <param name="cancellationToken">Token de cancelación de la tarea.</param>
    /// <returns>El DTO de la venta procesada con sus subtotales e impuestos calculados.</returns>
    /// <exception cref="ArgumentException">Se lanza si no hay ítems o si la información básica de la venta es inválida.</exception>
    /// <exception cref="InvalidOperationException">Se lanza si el repuesto no existe o no cuenta con stock suficiente.</exception>
    public async Task<VentaResponseDto> CrearVentaAsync(CrearVentaDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.Detalles == null || !dto.Detalles.Any())
            throw new ArgumentException("La venta debe contener al menos un producto.", nameof(dto.Detalles));

        Venta nuevaVenta = new Venta(dto.NumeroComprobante, dto.MetodoPago);

        foreach (CrearDetalleVentaDto item in dto.Detalles)
        {
            Producto? producto = await _productoRepository.GetByIdAsync(item.ProductoId, cancellationToken);
            if (producto == null)
            {
                throw new InvalidOperationException($"No se encontró el repuesto con ID: {item.ProductoId}");
            }

            if (producto.StockActual < item.Cantidad)
            {
                throw new InvalidOperationException(
                    $"Stock insuficiente para el repuesto '{producto.Nombre}'. Disponible: {producto.StockActual}, Solicitado: {item.Cantidad}");
            }

            // Descuento de inventario en el agregado del producto
            producto.AjustarStock(-item.Cantidad);
            await _productoRepository.UpdateAsync(producto, cancellationToken);

            // Agregar el detalle calculando subtotal e IVA
            nuevaVenta.AgregarDetalle(producto.Id, producto.Nombre, item.Cantidad, producto.PrecioVenta);
        }

        await _ventaRepository.AddAsync(nuevaVenta, cancellationToken);

        return VentaResponseDto.FromEntity(nuevaVenta);
    }
}