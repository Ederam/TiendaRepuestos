namespace TiendaRepuestos.Domain.Entities;

using System;

/// <summary>
/// Representa el ítem individual de un repuesto dentro de una transacción de venta.
/// </summary>
public class DetalleVenta
{
    public Guid Id { get; private set; }
    public Guid VentaId { get; private set; }
    public Guid ProductoId { get; private set; }
    public string NombreProducto { get; private set; } = string.Empty;
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal Subtotal { get; private set; }

    /// <summary>
    /// Constructor requerido por EF Core.
    /// </summary>
    private DetalleVenta() { }

    /// <summary>
    /// Inicializa una línea de detalle calculando automáticamente el subtotal.
    /// </summary>
    /// <param name="productoId">Identificador único del repuesto.</param>
    /// <param name="nombreProducto">Nombre o descripción del repuesto al momento de la venta.</param>
    /// <param name="cantidad">Unidades vendidas.</param>
    /// <param name="precioUnitario">Precio unitario congelado en la venta.</param>
    public DetalleVenta(Guid productoId, string nombreProducto, int cantidad, decimal precioUnitario)
    {
        if (productoId == Guid.Empty)
            throw new ArgumentException("El identificador del producto es obligatorio.", nameof(productoId));

        if (string.IsNullOrWhiteSpace(nombreProducto))
            throw new ArgumentException("El nombre del producto es obligatorio.", nameof(nombreProducto));

        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(cantidad));

        if (precioUnitario < 0)
            throw new ArgumentException("El precio unitario no puede ser negativo.", nameof(precioUnitario));

        Id = Guid.NewGuid();
        ProductoId = productoId;
        NombreProducto = nombreProducto.Trim();
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        Subtotal = cantidad * precioUnitario;
    }
}