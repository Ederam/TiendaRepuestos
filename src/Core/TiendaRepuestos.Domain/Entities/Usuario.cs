namespace TiendaRepuestos.Domain.Entities;

using System;
using TiendaRepuestos.Domain.Enums;

/// <summary>
/// Representa a un usuario autenticable del sistema POS.
/// </summary>
public class Usuario
{
    /// <summary>
    /// Identificador único del usuario.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    public string NombreCompleto { get; private set; }

    /// <summary>
    /// Correo electrónico único utilizado como identificador de login.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Hash seguro de la contraseña (BCrypt o PBKDF2). Nunca almacenar en texto plano.
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// Rol asignado para control de acceso basado en roles (RBAC).
    /// </summary>
    public RolUsuario Rol { get; private set; }

    /// <summary>
    /// Indica si la cuenta se encuentra habilitada para operar.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Constructor protegido requerido por Entity Framework Core.
    /// </summary>
    protected Usuario()
    {
        NombreCompleto = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="Usuario"/>.
    /// </summary>
    public Usuario(string nombreCompleto, string email, string passwordHash, RolUsuario rol)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            throw new ArgumentException("El nombre no puede ser vacío.", nameof(nombreCompleto));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email no puede ser vacío.", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de contraseña es obligatorio.", nameof(passwordHash));

        Id = Guid.NewGuid();
        NombreCompleto = nombreCompleto.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Rol = rol;
        Activo = true;
    }

    /// <summary>
    /// Desactiva el acceso del usuario al sistema.
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
    }
}