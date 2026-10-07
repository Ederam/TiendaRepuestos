namespace TiendaRepuestos.Infrastructure.Authentication;

using BCrypt.Net;
using TiendaRepuestos.Domain.Ports;

/// <summary>
/// Implementación de hashing de contraseñas utilizando el algoritmo BCrypt.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <inheritdoc />
    public string HashPassword(string plainPassword)
    {
        return BCrypt.HashPassword(plainPassword);
    }

    /// <inheritdoc />
    public bool VerifyPassword(string plainPassword, string passwordHash)
    {
        return BCrypt.Verify(plainPassword, passwordHash);
    }
}