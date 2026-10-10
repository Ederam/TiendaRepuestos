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
/// Implementación de infraestructura para la gestión de productos mediante Entity Framework Core y PostgreSQL.
/// </summary>
public class ProductoRepository : IProductoRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ProductoRepository"/>.
    /// </summary>
    /// <param name="context">Contexto principal de la base de datos.</param>
    /// <exception cref="ArgumentNullException">Se lanza si el contexto provisto es nulo.</exception>
    public ProductoRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<Producto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Productos
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Producto?> GetByCodigoParteAsync(string codigoParte, CancellationToken cancellationToken = default)
    {
        string codigoNormalizado = codigoParte.Trim().ToUpper();
        return await _context.Productos
            .FirstOrDefaultAsync(p => p.CodigoParte.ToUpper() == codigoNormalizado, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Producto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Productos
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Producto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await _context.Productos
                .AsNoTracking()
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .Take(20)
                .ToListAsync(cancellationToken);
        }

        string term = searchTerm.Trim().ToUpper();

        return await _context.Productos
            .AsNoTracking()
            .Where(p => p.Activo &&
                       (p.Nombre.ToUpper().Contains(term) || p.CodigoParte.ToUpper().Contains(term)))
            .OrderBy(p => p.Nombre)
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyCollection<Producto> Items, int TotalCount)> ObtenerPaginadoAsync(
        string? busqueda,
        Guid? categoriaId,
        bool? soloActivos,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Producto> query = _context.Productos.AsNoTracking();

        if (soloActivos.HasValue)
        {
            query = query.Where(p => p.Activo == soloActivos.Value);
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(p => p.CategoriaId == categoriaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string criterio = busqueda.Trim().ToLower();
            query = query.Where(p => p.Nombre.ToLower().Contains(criterio) || p.CodigoParte.ToLower().Contains(criterio));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        List<Producto> items = await query
            .OrderBy(p => p.Nombre)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items.AsReadOnly(), totalCount);
    }

    /// <inheritdoc/>
    public async Task AddAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        await _context.Productos.AddAsync(producto, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync(cancellationToken);
    }
}