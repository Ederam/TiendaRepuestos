namespace TiendaRepuestos.Application.DTOs.Reportes;

using System;

/// <summary>
/// Proyección inmutable con estadísticas de rendimiento de venta por repuesto.
/// </summary>
public record ReporteTopProductoDto
{
    /// <summary>
    /// Identificador único del producto vendido.
    /// </summary>
    public Guid ProductoId { get; init; }

    /// <summary>
    /// Nombre descriptivo del repuesto.
    /// </summary>
    public string NombreProducto { get; init; } = string.Empty;

    /// <summary>
    /// Cantidad consolidada de unidades vendidas.
    /// </summary>
    public int TotalUnidadesVendidas { get; init; }

    /// <summary>
    /// Monto monetario bruto acumulado por las ventas del repuesto.
    /// </summary>
    public decimal TotalRecaudado { get; init; }
}