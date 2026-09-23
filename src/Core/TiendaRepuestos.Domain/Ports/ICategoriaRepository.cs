namespace TiendaRepuestos.Domain.Ports;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Puerto secundario para las operaciones de persistencia del agregador Categoría.
/// </summary>
public interface ICategoriaRepository
{
    Task<Categoria?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Categoria>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Categoria categoria, CancellationToken cancellationToken = default);
    Task UpdateAsync(Categoria categoria, CancellationToken cancellationToken = default);
}