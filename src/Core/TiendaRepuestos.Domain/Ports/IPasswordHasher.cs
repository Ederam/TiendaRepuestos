namespace TiendaRepuestos.Domain.Ports;

/// <summary>
/// Contrato para el cálculo y verificación de hashes criptográficos de contraseñas.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Genera un hash seguro con sal incorporada para una contraseña en texto plano.
    /// </summary>
    string HashPassword(string plainPassword);

    /// <summary>
    /// Verifica si una contraseña en texto plano coincide con el hash almacenado.
    /// </summary>
    bool VerifyPassword(string plainPassword, string passwordHash);
}