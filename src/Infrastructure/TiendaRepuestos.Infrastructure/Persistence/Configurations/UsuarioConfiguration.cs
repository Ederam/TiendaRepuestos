namespace TiendaRepuestos.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Mapeo relacional de la entidad <see cref="Usuario"/> en la base de datos PostgreSQL.
/// </summary>
public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.NombreCompleto)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Rol)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.Activo)
            .IsRequired();
    }
}