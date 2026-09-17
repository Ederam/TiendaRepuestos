namespace TiendaRepuestos.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Configuración de mapeo Fluent API para la entidad <see cref="Producto"/> en la base de datos PostgreSQL.
/// </summary>
public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    /// <summary>
    /// Configura la estructura de la tabla, claves, índices y restricciones para la entidad Producto.
    /// </summary>
    /// <param name="builder">Constructor de la entidad proporcionado por EF Core.</param>
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("productos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.CodigoParte)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(p => p.PrecioVenta)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(p => p.StockActual)
            .IsRequired();

        builder.Property(p => p.StockMinimo)
            .IsRequired();

        builder.Property(p => p.Estado)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.Activo)
            .HasDefaultValue(true);

        // Índice de búsqueda rápida por Nombre y Código de Parte
        builder.HasIndex(p => p.Nombre);
        builder.HasIndex(p => p.CodigoParte);
    }
}