namespace TiendaRepuestos.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Ports;

/// <summary>
/// Implementación de persistencia para clientes con Entity Framework Core sobre PostgreSQL.
/// </summary>
public class ClienteRepository : IClienteRepository
{
    private readonly ApplicationDbContext _context;

    public ClienteRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Cliente>()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Cliente?> GetByDocumentoAsync(string documento, CancellationToken cancellationToken = default)
    {
        string docLimpio = documento.Trim().ToUpperInvariant();
        return await _context.Set<Cliente>()
            .FirstOrDefaultAsync(c => c.Documento == docLimpio, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Cliente>> SearchAsync(string termino, CancellationToken cancellationToken = default)
    {
        string termLimpio = termino.Trim().ToUpperInvariant();

        List<Cliente> items = await _context.Set<Cliente>()
            .AsNoTracking()
            .Where(c => c.Activo && (c.Documento.Contains(termLimpio) || c.Nombre.ToUpper().Contains(termLimpio)))
            .OrderBy(c => c.Nombre)
            .Take(15)
            .ToListAsync(cancellationToken);

        return items.AsReadOnly();
    }

    public async Task AddAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _context.Set<Cliente>().AddAsync(cliente, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        _context.Set<Cliente>().Update(cliente);
        await _context.SaveChangesAsync(cancellationToken);
    }
}