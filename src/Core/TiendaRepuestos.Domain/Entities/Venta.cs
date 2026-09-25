namespace TiendaRepuestos.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using TiendaRepuestos.Domain.Enums;

/// <summary>
/// Raíz del agregado que gestiona la transacción comercial de venta en el POS.
/// </summary>
public class Venta
{
    private readonly List<DetalleVenta> _detalles = new();

    public Guid Id { get; private set; }
    public string NumeroComprobante { get; private set; } = string.Empty;
    public DateTime FechaVenta { get; private set; }
    public MetodoPago MetodoPago { get; private set; }
    public EstadoVenta Estado { get; private set; }
    
    public decimal Subtotal { get; private set; }
    public decimal Impuesto { get; private set; }
    public decimal Total { get; private set; }

    public IReadOnlyCollection<DetalleVenta> Detalles => _detalles.AsReadOnly();

    /// <summary>
    /// Constructor privado requerido por EF Core.
    /// </summary>
    private Venta() { }

    /// <summary>
    /// Crea el encabezado de una nueva venta.
    /// </summary>
    /// <param name="numeroComprobante">Prefijo y consecutivo de factura/ticket (ej: VTA-0001).</param>
    /// <param name="metodoPago">Medio de pago utilizado.</param>
    public Venta(string numeroComprobante, MetodoPago metodoPago)
    {
        if (string.IsNullOrWhiteSpace(numeroComprobante))
            throw new ArgumentException("El número de comprobante es obligatorio.", nameof(numeroComprobante));

        Id = Guid.NewGuid();
        NumeroComprobante = numeroComprobante.Trim();
        FechaVenta = DateTime.UtcNow;
        MetodoPago = metodoPago;
        Estado = EstadoVenta.Completada;
    }

    /// <summary>
    /// Agrega una línea de producto a la venta y recalcula los totales.
    /// </summary>
    /// <param name="productoId">Identificador del producto.</param>
    /// <param name="nombreProducto">Nombre comercial del repuesto.</param>
    /// <param name="cantidad">Cantidad vendida.</param>
    /// <param name="precioUnitario">Precio congelado.</param>
    /// <param name="porcentajeImpuesto">Porcentaje de impuesto a aplicar (ej: 0.19 para 19% de IVA).</param>
    public void AgregarDetalle(Guid productoId, string nombreProducto, int cantidad, decimal precioUnitario, decimal porcentajeImpuesto = 0.19m)
    {
        if (Estado != EstadoVenta.Completada)
            throw new InvalidOperationException("No se pueden agregar ítems a una venta no activa.");

        var detalle = new DetalleVenta(productoId, nombreProducto, cantidad, precioUnitario);
        _detalles.Add(detalle);

        CalcularTotales(porcentajeImpuesto);
    }

    /// <summary>
    /// Recalcula el subtotal, impuesto y total general de la venta.
    /// </summary>
    private void CalcularTotales(decimal porcentajeImpuesto)
    {
        Subtotal = _detalles.Sum(d => d.Subtotal);
        Impuesto = Subtotal * porcentajeImpuesto;
        Total = Subtotal + Impuesto;
    }

    /// <summary>
    /// Anula la venta cambiando su estado.
    /// </summary>
    public void Anular()
    {
        if (Estado == EstadoVenta.Anulada)
            throw new InvalidOperationException("La venta ya se encuentra anulada.");

        Estado = EstadoVenta.Anulada;
    }
}