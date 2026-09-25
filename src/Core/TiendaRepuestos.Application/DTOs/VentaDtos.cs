namespace TiendaRepuestos.Application.DTOs;

using System;
using System.Collections.Generic;
using System.Linq;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Enums;

/// <summary>
/// DTO que representa un ítem individual dentro de la solicitud de creación de una venta.
/// </summary>
public class CrearDetalleVentaDto
{
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
}

/// <summary>
/// DTO de entrada para procesar una nueva venta desde la caja registradora (POS).
/// </summary>
public class CrearVentaDto
{
    public string NumeroComprobante { get; set; } = string.Empty;
    public MetodoPago MetodoPago { get; set; }
    public List<CrearDetalleVentaDto> Detalles { get; set; } = new();
}

/// <summary>
/// DTO de respuesta para presentar el detalle individual de un ítem vendido.
/// </summary>
public class DetalleVentaResponseDto
{
    public Guid Id { get; set; }
    public Guid ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

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
/// DTO de salida para devolver la información completa del comprobante de venta.
/// </summary>
public class VentaResponseDto
{
    public Guid Id { get; set; }
    public string NumeroComprobante { get; set; } = string.Empty;
    public DateTime FechaVenta { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Total { get; set; }
    public List<DetalleVentaResponseDto> Detalles { get; set; } = new();

    public static VentaResponseDto FromEntity(Venta venta)
    {
        return new VentaResponseDto
        {
            Id = venta.Id,
            NumeroComprobante = venta.NumeroComprobante,
            FechaVenta = venta.FechaVenta,
            MetodoPago = venta.MetodoPago.ToString(),
            Estado = venta.Estado.ToString(),
            Subtotal = venta.Subtotal,
            Impuesto = venta.Impuesto,
            Total = venta.Total,
            Detalles = venta.Detalles.Select(DetalleVentaResponseDto.FromEntity).ToList()
        };
    }
}