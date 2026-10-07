namespace TiendaRepuestos.Domain.Ports;

using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Puerto para la generación y firma de tokens de acceso JWT.
/// </summary>
public interface IJwtGenerator
{
    /// <summary>
    /// Genera un token JWT firmado conteniendo los claims de identidad y rol del usuario.
    /// </summary>
    /// <param name="usuario">Entidad del usuario autenticado.</param>
    /// <returns>El token JWT serializado en formato string.</returns>
    string GenerarToken(Usuario usuario);
}