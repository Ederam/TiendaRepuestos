namespace TiendaRepuestos.Application.DTOs.Auth;

using TiendaRepuestos.Domain.Enums;

/// <summary>
/// Parámetros requeridos para la creación de un nuevo usuario en el sistema.
/// </summary>
public class RegistroUsuarioDto
{
    /// <summary>
    /// Nombre y apellido del usuario.
    /// </summary>
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Dirección de correo electrónico única.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña propuesta.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Rol asignado al nuevo usuario.
    /// </summary>
    public RolUsuario Rol { get; set; }
}