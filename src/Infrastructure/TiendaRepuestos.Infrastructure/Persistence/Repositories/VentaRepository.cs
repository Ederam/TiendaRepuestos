namespace TiendaRepuestos.Infrastructure.Persistence.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Models;
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
    /// Obtiene el historial de ventas con LEFT JOIN a usuarios para resolver el nombre del vendedor.
    /// Equivale a: SELECT v.*, u."NombreCompleto" FROM "Ventas" v LEFT JOIN usuarios u ON u."Id" = v."UsuarioId"
    /// [WHERE v."UsuarioId" = @usuarioId] ORDER BY v."FechaVenta" DESC.
    /// </summary>
    public async Task<IReadOnlyList<VentaConVendedor>> ObtenerHistorialAsync(Guid? usuarioId, CancellationToken cancellationToken = default)
    {
        IQueryable<Venta> ventas = _context.Ventas
            .AsNoTracking()
            .Include(v => v.Detalles);

        if (usuarioId.HasValue)
        {
            ventas = ventas.Where(v => v.UsuarioId == usuarioId.Value);
        }

        var filas = await (
            from v in ventas
            join u in _context.Usuarios.AsNoTracking() on v.UsuarioId equals (Guid?)u.Id into vendedores
            from u in vendedores.DefaultIfEmpty()
            orderby v.FechaVenta descending
            select new { Venta = v, NombreVendedor = u != null ? u.NombreCompleto : null }
        ).ToListAsync(cancellationToken);

        return filas.Select(f => new VentaConVendedor(f.Venta, f.NombreVendedor)).ToList();
    }

    /// <summary>
    /// Obtiene una venta por su identificador junto con el nombre del vendedor (LEFT JOIN a usuarios).
    /// </summary>
    public async Task<VentaConVendedor?> ObtenerConVendedorPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var fila = await (
            from v in _context.Ventas.AsNoTracking().Include(v => v.Detalles)
            where v.Id == id
            join u in _context.Usuarios.AsNoTracking() on v.UsuarioId equals (Guid?)u.Id into vendedores
            from u in vendedores.DefaultIfEmpty()
            select new { Venta = v, NombreVendedor = u != null ? u.NombreCompleto : null }
        ).FirstOrDefaultAsync(cancellationToken);

        return fila is null ? null : new VentaConVendedor(fila.Venta, fila.NombreVendedor);
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