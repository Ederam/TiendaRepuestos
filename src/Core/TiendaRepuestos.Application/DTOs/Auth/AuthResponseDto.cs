namespace TiendaRepuestos.Application.DTOs.Auth;

using System;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Información de credenciales y datos de sesión retornados tras una autenticación exitosa.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// Identificador del usuario.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico registrado.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Rol asignado al usuario.
    /// </summary>
    public string Rol { get; set; } = string.Empty;

    /// <summary>
    /// Token JWT firmado para autorizar peticiones subsecuentes en cabecera Bearer.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Construye una respuesta de autenticación a partir de la entidad y el token generado.
    /// </summary>
    public static AuthResponseDto Create(Usuario usuario, string token)
    {
        return new AuthResponseDto
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString(),
            Token = token
        };
    }
}