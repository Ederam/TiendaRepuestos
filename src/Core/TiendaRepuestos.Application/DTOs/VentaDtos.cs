namespace TiendaRepuestos.Application.DTOs;

using System;
using System.Collections.Generic;
using System.Linq;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Enums;

/// <summary>
/// Estructura de entrada para asociar un producto y su volumen en la solicitud de venta.
/// </summary>
public class CrearDetalleVentaDto
{
    /// <summary>
    /// Identificador único (GUID) del repuesto a descargar de inventario.
    /// </summary>
    public Guid ProductoId { get; set; }

    /// <summary>
    /// Cantidad de unidades a adquirir.
    /// </summary>
    public int Cantidad { get; set; }
}

/// <summary>
/// Parámetros de entrada para el procesamiento y confirmación de una orden en caja registradora (POS).
/// No incluye el consecutivo, ya que es adjudicado de manera atómica por el motor transaccional.
/// </summary>
public class CrearVentaDto
{
    /// <summary>
    /// Modalidad con la que se efectúa el pago (1: Efectivo, 2: Transferencia, 3: Tarjeta).
    /// </summary>
    public MetodoPago MetodoPago { get; set; }

    /// <summary>
    /// Identificador único del cliente si está registrado en el directorio. Opcional.
    /// </summary>
    public Guid? ClienteId { get; set; }

    /// <summary>
    /// Número de cédula o NIT de facturación. Si es nulo o vacío se asigna 222222222222.
    /// </summary>
    public string? ClienteDocumento { get; set; }

    /// <summary>
    /// Nombre o razón social del comprador. Si es nulo o vacío se asigna Consumidor Final.
    /// </summary>
    public string? ClienteNombre { get; set; }

    /// <summary>
    /// Listado de productos e ítems a facturar.
    /// </summary>
    public List<CrearDetalleVentaDto> Detalles { get; set; } = new();
}

/// <summary>
/// Estructura de salida que desglosa el contenido individual de un producto vendido.
/// </summary>
public class DetalleVentaResponseDto
{
    /// <summary>
    /// Identificador único del registro de detalle.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador único del repuesto vendido.
    /// </summary>
    public Guid ProductoId { get; set; }

    /// <summary>
    /// Nombre del repuesto en el momento exacto de la venta.
    /// </summary>
    public string NombreProducto { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad de unidades facturadas.
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Precio unitario aplicado a la transacción.
    /// </summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Subtotal calculado (Cantidad * PrecioUnitario).
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Transforma una entidad de dominio <see cref="DetalleVenta"/> a su representación DTO.
    /// </summary>
    /// <param name="detalle">Entidad de detalle persistida.</param>
    /// <returns>Instancia inmutable de <see cref="DetalleVentaResponseDto"/>.</returns>
    public static DetalleVentaResponseDto FromEntity(DetalleVenta detalle)
    {
        return new DetalleVentaResponseDto
        {
            Id = detalle.Id,
            ProductoId = detalle.ProductoId,
            NombreProducto = detalle.NombreProducto,
            Cantidad = detalle.Cantidad,
            PrecioUnitario = detalle.PrecioUnitario,
            Subtotal = detalle.Subtotal
        };
    }
}

/// <summary>
/// Estructura de salida con los datos fiscales, financieros y de trazabilidad del comprobante de venta emitido.
/// </summary>
public class VentaResponseDto
{
    /// <summary>
    /// Identificador único (GUID) de la venta.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Consecutivo generado oficialmente por el sistema (ejemplo: VTA-00001).
    /// </summary>
    public string NumeroComprobante { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora en UTC en que se procesó la venta.
    /// </summary>
    public DateTime FechaVenta { get; set; }

    /// <summary>
    /// Método de pago utilizado en formato textual.
    /// </summary>
    public string MetodoPago { get; set; } = string.Empty;

    /// <summary>
    /// Estado transaccional del comprobante.
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del cliente asociado (si existe).
    /// </summary>
    public Guid? ClienteId { get; set; }

    /// <summary>
    /// Documento o NIT registrado para el comprobante.
    /// </summary>
    public string ClienteDocumento { get; set; } = string.Empty;

    /// <summary>
    /// Razón social o nombre completo del comprador.
    /// </summary>
    public string ClienteNombre { get; set; } = string.Empty;

    /// <summary>
    /// Monto bruto acumulado de la venta.
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Impuesto liquidado aplicable.
    /// </summary>
    public decimal Impuesto { get; set; }

    /// <summary>
    /// Monto total de la operación.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Desglose de repuestos incluidos en la factura.
    /// </summary>
    public List<DetalleVentaResponseDto> Detalles { get; set; } = new();

    /// <summary>
    /// Transforma una entidad de dominio <see cref="Venta"/> a su representación pública DTO.
    /// </summary>
    /// <param name="venta">Entidad de agregado raíz persistida.</param>
    /// <returns>Instancia serializable de <see cref="VentaResponseDto"/>.</returns>
    public static VentaResponseDto FromEntity(Venta venta)
    {
        return new VentaResponseDto
        {
            Id = venta.Id,
            NumeroComprobante = venta.NumeroComprobante,
            FechaVenta = venta.FechaVenta,
            MetodoPago = venta.MetodoPago.ToString(),
            Estado = venta.Estado.ToString(),
            ClienteId = venta.ClienteId,
            ClienteDocumento = venta.ClienteDocumento,
            ClienteNombre = venta.ClienteNombre,
            Subtotal = venta.Subtotal,
            Impuesto = venta.Impuesto,
            Total = venta.Total,
            Detalles = venta.Detalles.Select(DetalleVentaResponseDto.FromEntity).ToList()
        };
    }
}