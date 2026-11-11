namespace TiendaRepuestos.Domain.Ports;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Models;

/// <summary>
/// Puerto secundario para las operaciones de persistencia del agregado Venta.
/// </summary>
public interface IVentaRepository
{
    Task<Venta?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Venta>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el historial de ventas con el nombre del vendedor (JOIN con usuarios).
    /// </summary>
    /// <param name="usuarioId">Si se informa, filtra estrictamente por el usuario emisor; si es null, retorna el consolidado.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    Task<IReadOnlyList<VentaConVendedor>> ObtenerHistorialAsync(Guid? usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una venta por su identificador junto con el nombre del vendedor que la emitió.
    /// </summary>
    Task<VentaConVendedor?> ObtenerConVendedorPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Venta venta, CancellationToken cancellationToken = default);
}
