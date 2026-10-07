namespace TiendaRepuestos.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Contexto principal de Entity Framework Core para la aplicación de Tienda de Repuestos.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ApplicationDbContext"/>.
    /// </summary>
    /// <param name="options">Opciones de configuración del DbContext.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Conjunto de entidades de productos en inventario.
    /// </summary>
    public DbSet<Producto> Productos => Set<Producto>();

    /// <summary>
    /// Conjunto de entidades de categorías de repuestos.
    /// </summary>
    public DbSet<Categoria> Categorias => Set<Categoria>();

    /// <summary>
    /// Conjunto de entidades de ventas generadas.
    /// </summary>
    public DbSet<Venta> Ventas => Set<Venta>();

    /// <summary>
    /// Conjunto de entidades de detalles de venta.
    /// </summary>
    public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();

    /// <summary>
    /// Conjunto de entidades de usuarios para autenticación y autorización.
    /// </summary>
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    /// <summary>
    /// Configura las entidades del modelo aplicando todas las configuraciones Fluent API del ensamblado.
    /// </summary>
    /// <param name="modelBuilder">Constructor de modelos de Entity Framework.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}