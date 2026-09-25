namespace TiendaRepuestos.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Configuración de mapeo para la entidad <see cref="Venta"/> en EF Core y PostgreSQL.
/// </summary>
public class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    /// <summary>
    /// Configura las propiedades del encabezado de venta y la relación uno a muchos con sus detalles.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("Ventas");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.NumeroComprobante)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.FechaVenta)
            .IsRequired();

        builder.Property(v => v.MetodoPago)
            .IsRequired();

        builder.Property(v => v.Estado)
            .IsRequired();

        builder.Property(v => v.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(v => v.Impuesto)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(v => v.Total)
            .IsRequired()
            .HasPrecision(18, 2);

        // Configuración de la relación uno a muchos con la colección de detalles privada
        builder.HasMany(v => v.Detalles)
            .WithOne()
            .HasForeignKey(d => d.VentaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}