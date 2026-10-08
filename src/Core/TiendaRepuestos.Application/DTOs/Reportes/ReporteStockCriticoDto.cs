namespace TiendaRepuestos.Application.DTOs.Reportes;

using System;

/// <summary>
/// Proyección inmutable para reportes de repuestos con niveles de inventario en umbral crítico.
/// </summary>
public record ReporteStockCriticoDto
{
    /// <summary>
    /// Identificador único del repuesto.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Nombre comercial del repuesto.
    /// </summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Código de parte o referencia técnica del fabricante.
    /// </summary>
    public string CodigoParte { get; init; } = string.Empty;

    /// <summary>
    /// Nombre de la categoría a la que pertenece el producto.
    /// </summary>
    public string Categoria { get; init; } = string.Empty;

    /// <summary>
    /// Existencias actuales en bodega o mostrador.
    /// </summary>
    public int StockActual { get; init; }

    /// <summary>
    /// Nivel mínimo parametrizado antes de requerir reabastecimiento.
    /// </summary>
    public int StockMinimo { get; init; }

    /// <summary>
    /// Diferencia o déficit respecto al stock mínimo.
    /// </summary>
    public int Deficit => StockMinimo - StockActual;
}