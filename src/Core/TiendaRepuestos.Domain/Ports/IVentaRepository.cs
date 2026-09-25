namespace TiendaRepuestos.Domain.Ports;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Puerto secundario para las operaciones de persistencia del agregado Venta.
/// </summary>
public interface IVentaRepository
{
    Task<Venta?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Venta>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Venta venta, CancellationToken cancellationToken = default);
}