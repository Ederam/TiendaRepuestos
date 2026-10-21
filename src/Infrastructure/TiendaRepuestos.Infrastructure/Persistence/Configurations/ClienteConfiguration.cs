namespace TiendaRepuestos.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Configuración de mapeo relacional en PostgreSQL para la entidad <see cref="Cliente"/>.
/// </summary>
public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Documento)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Telefono)
            .HasMaxLength(30);

        builder.Property(c => c.Correo)
            .HasMaxLength(120);

        builder.Property(c => c.Direccion)
            .HasMaxLength(200);

        builder.Property(c => c.EsFacturaElectronica)
            .IsRequired();

        builder.Property(c => c.Activo)
            .IsRequired();

        // Índice único por documento para evitar duplicidad
        builder.HasIndex(c => c.Documento)
            .IsUnique()
            .HasDatabaseName("IX_Clientes_Documento");
    }
}