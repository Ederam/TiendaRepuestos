namespace TiendaRepuestos.Infrastructure.Persistence.Repositories;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Ports;

/// <summary>
/// Implementación concreta del puerto <see cref="IVentaRepository"/> utilizando Entity Framework Core.
/// </summary>
public class VentaRepository : IVentaRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VentaRepository"/>.
    /// </summary>
    /// <param name="context">Contexto principal de base de datos.</param>
    public VentaRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Obtiene una venta por su identificador único incluyendo su colección de detalles.
    /// </summary>
    /// <param name="id">Identificador único (GUID) de la venta.</param>
    /// <param name="cancellationToken">Token para la cancelación de la tarea asíncrona.</param>
    /// <returns>La entidad <see cref="Venta"/> si se encuentra; de lo contrario, null.</returns>
    public async Task<Venta?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Ventas
            .Include(v => v.Detalles)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    /// <summary>
    /// Obtiene el historial completo de ventas con sus detalles en modo solo lectura.
    /// </summary>
    /// <param name="cancellationToken">Token para la cancelación de la tarea asíncrona.</param>
    /// <returns>Colección de ventas procesadas.</returns>
    public async Task<IEnumerable<Venta>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Ventas
            .Include(v => v.Detalles)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Agrega una nueva transacción de venta a la base de datos.
    /// </summary>
    /// <param name="venta">Instancia del agregado Venta a guardar.</param>
    /// <param name="cancellationToken">Token para la cancelación de la tarea asíncrona.</param>
    public async Task AddAsync(Venta venta, CancellationToken cancellationToken = default)
    {
        await _context.Ventas.AddAsync(venta, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}