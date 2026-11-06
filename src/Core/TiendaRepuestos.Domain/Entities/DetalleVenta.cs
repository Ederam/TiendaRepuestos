namespace TiendaRepuestos.Domain.Entities;

using System;

/// <summary>
/// Representa un ítem o renglón individual dentro de un comprobante de venta.
/// Mantiene la inmutabilidad del precio histórico de venta y el cálculo de subtotales.
/// </summary>
public class DetalleVenta
{
    /// <summary>
    /// Identificador único global (UUID/GUID) del registro de detalle.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identificador de la venta principal a la que pertenece este detalle (Clave foránea).
    /// </summary>
    public Guid VentaId { get; private set; }

    /// <summary>
    /// Identificador único del producto o repuesto vendido.
    /// </summary>
    public Guid ProductoId { get; private set; }

    /// <summary>
    /// Snapshot inmutable del nombre del producto al momento de realizar la transacción comercial.
    /// </summary>
    public string NombreProducto { get; private set; } = string.Empty;

    /// <summary>
    /// Cantidad de unidades comercializadas.
    /// </summary>
    public int Cantidad { get; private set; }

    /// <summary>
    /// Precio unitario de venta congelado al momento de la facturación.
    /// </summary>
    public decimal PrecioUnitario { get; private set; }

    /// <summary>
    /// Descuento unitario aplicado a este ítem (por defecto 0).
    /// </summary>
    public decimal Descuento { get; private set; }

    /// <summary>
    /// Subtotal neto del ítem calculado como ((PrecioUnitario - Descuento) * Cantidad).
    /// </summary>
    public decimal Subtotal { get; private set; }

    /// <summary>
    /// Constructor protegido sin parámetros requerido por el motor de mapeo de Entity Framework Core.
    /// </summary>
    protected DetalleVenta() { }

    /// <summary>
    /// Sobrecarga estándar de 4 argumentos para compatibilidad histórica con llamadas sin descuento.
    /// </summary>
    /// <param name="ventaId">Identificador único de la cabecera de venta.</param>
    /// <param name="productoId">Identificador del producto inventariado.</param>
    /// <param name="nombreProducto">Nombre descriptivo del repuesto.</param>
    /// <param name="cantidad">Unidades vendidas.</param>
    /// <param name="precioUnitario">Precio unitario de venta.</param>
    public DetalleVenta(Guid ventaId, Guid productoId, string nombreProducto, int cantidad, decimal precioUnitario)
        : this(ventaId, productoId, nombreProducto, cantidad, precioUnitario, 0m)
    {
    }

    /// <summary>
    /// Constructor canónico de 6 argumentos que inicializa el detalle con descuento unitario explícito.
    /// </summary>
    /// <param name="ventaId">Identificador de la transacción de venta.</param>
    /// <param name="productoId">Identificador del producto inventariado.</param>
    /// <param name="nombreProducto">Nombre descriptivo del repuesto.</param>
    /// <param name="cantidad">Unidades vendidas.</param>
    /// <param name="precioUnitario">Precio unitario congelado.</param>
    /// <param name="descuento">Monto de descuento unitario aplicado.</param>
    public DetalleVenta(Guid ventaId, Guid productoId, string nombreProducto, int cantidad, decimal precioUnitario, decimal descuento)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad debe ser estrictamente mayor a cero.");
        }

        if (precioUnitario < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(precioUnitario), "El precio unitario no puede ser negativo.");
        }

        if (descuento < 0 || descuento > precioUnitario)
        {
            throw new ArgumentOutOfRangeException(nameof(descuento), "El descuento no puede ser negativo ni superior al precio unitario.");
        }

        Id = Guid.NewGuid();
        VentaId = ventaId;
        ProductoId = productoId;
        NombreProducto = string.IsNullOrWhiteSpace(nombreProducto)
            ? throw new ArgumentException("El nombre del producto es requerido.", nameof(nombreProducto))
            : nombreProducto.Trim();
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        Descuento = descuento;
        Subtotal = (precioUnitario - descuento) * cantidad;
    }
}