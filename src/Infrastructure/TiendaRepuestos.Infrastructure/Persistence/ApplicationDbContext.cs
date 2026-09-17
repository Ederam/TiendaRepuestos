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
    /// Obtiene o establece el conjunto de datos de los productos/repuestos.
    /// </summary>
    public DbSet<Producto> Productos => Set<Producto>();

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