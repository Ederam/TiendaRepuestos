namespace TiendaRepuestos.Application.DTOs.Auth;

/// <summary>
/// Parámetros requeridos para la autenticación de un usuario en el sistema.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Correo electrónico registrado.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña en texto plano suministrada por el usuario.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}