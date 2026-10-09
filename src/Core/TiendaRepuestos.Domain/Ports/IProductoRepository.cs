namespace TiendaRepuestos.Domain.Ports;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Puerto secundario que define las operaciones de persistencia y consulta para la entidad <see cref="Producto"/>.
/// </summary>
public interface IProductoRepository
{
    /// <summary>
    /// Obtiene un repuesto por su identificador único (GUID).
    /// </summary>
    /// <param name="id">Identificador único del repuesto.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>La entidad encontrada o null si no existe.</returns>
    Task<Producto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un repuesto por su código de parte técnico.
    /// </summary>
    /// <param name="codigoParte">Código técnico asignado por fabricante.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>La entidad encontrada o null si no existe.</returns>
    Task<Producto?> GetByCodigoParteAsync(string codigoParte, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la totalidad de los repuestos registrados en el inventario.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Colección completa de repuestos.</returns>
    Task<IEnumerable<Producto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca productos mediante coincidencia en nombre o código de parte.
    /// </summary>
    /// <param name="searchTerm">Criterio textual de búsqueda.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Colección de productos encontrados.</returns>
    Task<IEnumerable<Producto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una lista paginada y filtrada de productos junto con el conteo total de coincidencias.
    /// </summary>
    /// <param name="busqueda">Texto de búsqueda sobre nombre o código de parte.</param>
    /// <param name="categoriaId">Identificador opcional de la categoría.</param>
    /// <param name="soloActivos">Indica si solo deben incluirse productos activos.</param>
    /// <param name="pageNumber">Número de página a consultar.</param>
    /// <param name="pageSize">Cantidad de registros por página.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Tupla con los productos de la página y el total absoluto de registros encontrados.</returns>
    Task<(IReadOnlyCollection<Producto> Items, int TotalCount)> ObtenerPaginadoAsync(
        string? busqueda,
        Guid? categoriaId,
        bool? soloActivos,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra un nuevo repuesto en el almacén de datos.
    /// </summary>
    /// <param name="producto">Entidad a persistir.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    Task AddAsync(Producto producto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza los datos de un repuesto existente.
    /// </summary>
    /// <param name="producto">Entidad modificada.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    Task UpdateAsync(Producto producto, CancellationToken cancellationToken = default);
}