namespace TiendaRepuestos.Domain.Entities;

using System;
using System.Collections.Generic;
using TiendaRepuestos.Domain.Enums;

/// <summary>
/// Agregado raíz que representa una transacción comercial de venta en el mostrador o facturación electrónica.
/// Gestiona su propio ciclo de vida, snapshot del cliente asociado y el cálculo estricto de totales.
/// </summary>
public class Venta
{
    /// <summary>
    /// Identificador único global (UUID/GUID) de la venta.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Código consecutivo oficial del comprobante emitido (ejemplo: VTA-00001).
    /// </summary>
    public string NumeroComprobante { get; private set; } = string.Empty;

    /// <summary>
    /// Fecha y hora UTC en la que se confirmó y procesó la venta.
    /// </summary>
    public DateTime FechaVenta { get; private set; }

    /// <summary>
    /// Modalidad de pago utilizada por el cliente (Efectivo, Transferencia, Tarjeta).
    /// </summary>
    public MetodoPago MetodoPago { get; private set; }

    /// <summary>
    /// Estado actual de la transacción (Completada, Anulada).
    /// </summary>
    public EstadoVenta Estado { get; private set; }

    /// <summary>
    /// Identificador foráneo opcional del cliente en el sistema. Null si es mostrador no registrado.
    /// </summary>
    public Guid? ClienteId { get; private set; }

    /// <summary>
    /// Snapshot inmutable del documento o NIT del comprador al momento de emitir la factura.
    /// </summary>
    public string ClienteDocumento { get; private set; } = string.Empty;

    /// <summary>
    /// Snapshot inmutable del nombre o razón social del comprador al momento de la venta.
    /// </summary>
    public string ClienteNombre { get; private set; } = string.Empty;

    /// <summary>
    /// Sumatoria del valor bruto de todos los ítems antes de descuentos e impuestos.
    /// </summary>
    public decimal Subtotal { get; private set; }

    /// <summary>
    /// Monto liquidado de impuestos (IVA) aplicable a la transacción.
    /// </summary>
    public decimal Impuesto { get; private set; }

    /// <summary>
    /// Monto neto total a pagar tras aplicar descuentos e impuestos.
    /// </summary>
    public decimal Total { get; private set; }

    private readonly List<DetalleVenta> _detalles = new();

    /// <summary>
    /// Colección de solo lectura con los renglones y repuestos incluidos en la venta.
    /// </summary>
    public IReadOnlyCollection<DetalleVenta> Detalles => _detalles.AsReadOnly();

    /// <summary>
    /// Constructor protegido sin parámetros requerido por el motor de mapeo de Entity Framework Core.
    /// </summary>
    protected Venta() { }

    /// <summary>
    /// Inicializa una nueva instancia de la transacción de venta con sus datos fiscales base.
    /// </summary>
    /// <param name="numeroComprobante">Consecutivo secuencial emitido.</param>
    /// <param name="metodoPago">Método de pago liquidado.</param>
    /// <param name="clienteId">Identificador único del cliente registrado (si aplica).</param>
    /// <param name="clienteDocumento">Número de identificación o NIT del comprador.</param>
    /// <param name="clienteNombre">Nombre o razón social del comprador.</param>
    public Venta(
        string numeroComprobante,
        MetodoPago metodoPago,
        Guid? clienteId,
        string? clienteDocumento,
        string? clienteNombre)
    {
        Id = Guid.NewGuid();
        NumeroComprobante = string.IsNullOrWhiteSpace(numeroComprobante)
            ? throw new ArgumentException("El número de comprobante es requerido.", nameof(numeroComprobante))
            : numeroComprobante;
        FechaVenta = DateTime.UtcNow;
        MetodoPago = metodoPago;
        Estado = EstadoVenta.Completada;
        ClienteId = clienteId;
        ClienteDocumento = string.IsNullOrWhiteSpace(clienteDocumento) ? "222222222222" : clienteDocumento.Trim();
        ClienteNombre = string.IsNullOrWhiteSpace(clienteNombre) ? "Consumidor Final" : clienteNombre.Trim();
        Subtotal = 0;
        Impuesto = 0;
        Total = 0;
    }

    /// <summary>
    /// Incorpora un nuevo repuesto al detalle de la venta recalculando los totales de forma encapsulada.
    /// </summary>
    /// <param name="productoId">Identificador del producto inventariado.</param>
    /// <param name="nombreProducto">Nombre oficial del repuesto.</param>
    /// <param name="cantidad">Unidades vendidas.</param>
    /// <param name="precioUnitario">Precio unitario congelado para la venta.</param>
    /// <param name="descuento">Monto de descuento unitario aplicado (opcional, por defecto 0).</param>
    public void AgregarDetalle(Guid productoId, string nombreProducto, int cantidad, decimal precioUnitario, decimal descuento = 0m)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad vendida debe ser mayor a cero.");
        }

        if (precioUnitario < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(precioUnitario), "El precio unitario no puede ser negativo.");
        }

        var detalle = new DetalleVenta(Id, productoId, nombreProducto, cantidad, precioUnitario, descuento);
        _detalles.Add(detalle);
        RecalcularTotales();
    }

    /// <summary>
    /// Realiza la consolidación matemática de subtotales y total neto de la venta.
    /// </summary>
    private void RecalcularTotales()
    {
        decimal acumuladoSubtotal = 0;

        foreach (var detalle in _detalles)
        {
            acumuladoSubtotal += detalle.Subtotal;
        }

        Subtotal = acumuladoSubtotal;
        Impuesto = 0; // Comprobante POS régimen mostrador
        Total = Subtotal + Impuesto;
    }
}