namespace TiendaRepuestos.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Configuración de mapeo para la entidad <see cref="DetalleVenta"/> en EF Core y PostgreSQL.
/// </summary>
public class DetalleVentaConfiguration : IEntityTypeConfiguration<DetalleVenta>
{
    /// <summary>
    /// Configura las restricciones de columnas y llaves foráneas para la tabla DetallesVenta.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<DetalleVenta> builder)
    {
        builder.ToTable("DetallesVenta");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.NombreProducto)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.Cantidad)
            .IsRequired();

        builder.Property(d => d.PrecioUnitario)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(d => d.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);
    }
}