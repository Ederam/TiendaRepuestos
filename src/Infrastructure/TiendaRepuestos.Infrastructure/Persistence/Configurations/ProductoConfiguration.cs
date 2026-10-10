namespace TiendaRepuestos.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Configuración de mapeo relacional (Fluent API) para la entidad <see cref="Producto"/> en PostgreSQL.
/// </summary>
public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("productos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.CodigoParte)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.CodigoParte)
            .IsUnique();

        builder.Property(p => p.PrecioVenta)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.StockActual)
            .IsRequired();

        builder.Property(p => p.StockMinimo)
            .IsRequired();

        builder.Property(p => p.Activo)
            .IsRequired();

        builder.Property(p => p.CategoriaId)
            .IsRequired();

        // 🚀 Concurrencia Optimista Nativa de PostgreSQL
        // EF Core utiliza la columna interna de sistema 'xmin' como RowVersion automático.
        builder.UseXminAsConcurrencyToken();
    }
}