namespace TiendaRepuestos.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Ports;

/// <summary>
/// Implementación de infraestructura para la gestión de productos mediante Entity Framework Core.
/// </summary>
public class ProductoRepository : IProductoRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ProductoRepository"/>.
    /// </summary>
    /// <param name="context">Contexto principal de la base de datos.</param>
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
    public async Task<IEnumerable<Producto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await _context.Productos
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .Take(20)
                .ToListAsync(cancellationToken);
        }

        var term = searchTerm.Trim().ToUpper();

        return await _context.Productos
            .Where(p => p.Activo &&
                       (p.Nombre.ToUpper().Contains(term) || p.CodigoParte.ToUpper().Contains(term)))
            .OrderBy(p => p.Nombre)
            .Take(50)
            .ToListAsync(cancellationToken);
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