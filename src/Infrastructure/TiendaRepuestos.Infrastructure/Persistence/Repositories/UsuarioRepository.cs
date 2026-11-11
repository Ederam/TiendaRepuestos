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
/// Repositorio para la gestión de usuarios sobre PostgreSQL usando EF Core.
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="UsuarioRepository"/>.
    /// </summary>
    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim(), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Usuario>> ObtenerVendedoresActivosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Activo)
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}