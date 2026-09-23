namespace TiendaRepuestos.Infrastructure.Persistence.Repositories;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Ports;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly ApplicationDbContext _context;

    public CategoriaRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Categoria?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Categoria>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categorias.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Categoria categoria, CancellationToken cancellationToken = default)
    {
        await _context.Categorias.AddAsync(categoria, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Categoria categoria, CancellationToken cancellationToken = default)
    {
        _context.Categorias.Update(categoria);
        await _context.SaveChangesAsync(cancellationToken);
    }
}