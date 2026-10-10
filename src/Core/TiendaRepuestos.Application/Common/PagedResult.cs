namespace TiendaRepuestos.Application.Common;

using System;
using System.Collections.Generic;

/// <summary>
/// Envoltorio genérico inmutable que encapsula un conjunto de elementos paginados y sus metadatos de navegación.
/// </summary>
/// <typeparam name="T">Tipo de dato de los elementos contenidos en la página actual.</typeparam>
public record PagedResult<T>
{
    /// <summary>
    /// Elementos correspondientes a la página actual solicitada.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>
    /// Número de la página actual (base 1).
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Cantidad máxima de registros por página.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Conteo total de elementos existentes que cumplen con los filtros aplicados.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Cantidad total de páginas calculadas.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Indica si existe una página anterior disponible.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indica si existe una página siguiente disponible.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="PagedResult{T}"/>.
    /// </summary>
    public PagedResult(IReadOnlyCollection<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}