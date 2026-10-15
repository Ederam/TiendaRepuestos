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

        builder.Property(p => p.Origen)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Ubicacion)
            .HasMaxLength(100);

        builder.Property(p => p.MarcaRepuesto)
            .HasMaxLength(100);

        builder.Property(p => p.MarcaVehiculo)
            .HasMaxLength(100);

        builder.Property(p => p.Condicion)
            .HasMaxLength(50);

        builder.Property(p => p.PrecioVenta)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Costo)
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

        // Clave de negocio única compuesta para permitir referencias compartidas con aliados
        builder.HasIndex(p => new { p.CodigoParte, p.Origen })
            .IsUnique()
            .HasDatabaseName("IX_Productos_CodigoParte_Origen");

        // Concurrencia Optimista Estándar en EF Core 8+ para PostgreSQL (elimina CS0618)
        builder.Property<uint>("xmin").IsRowVersion();
    }
}