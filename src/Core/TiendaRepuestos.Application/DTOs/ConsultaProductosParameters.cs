namespace TiendaRepuestos.Application.DTOs;

using System;

/// <summary>
/// Parámetros de entrada para la búsqueda, filtrado y paginación del catálogo de repuestos.
/// </summary>
public record ConsultaProductosParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Número de página solicitado (por defecto 1).
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Cantidad de registros por página (por defecto 10, máximo 100).
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
    }

    /// <summary>
    /// Término de búsqueda textual sobre el nombre o código de parte.
    /// </summary>
    public string? Busqueda { get; init; }

    /// <summary>
    /// Identificador único de categoría para filtrar repuestos específicos.
    /// </summary>
    public Guid? CategoriaId { get; init; }

    /// <summary>
    /// Filtro opcional por estado activo/inactivo del repuesto.
    /// </summary>
    public bool? SoloActivos { get; init; } = true;
}