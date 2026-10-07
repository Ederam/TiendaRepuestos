namespace TiendaRepuestos.Infrastructure.Authentication;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Ports;

/// <summary>
/// Implementación concreta del generador de tokens de acceso en formato JWT con claims estándar.
/// </summary>
public class JwtGenerator : IJwtGenerator
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="JwtGenerator"/>.
    /// </summary>
    /// <param name="configuration">Acceso a las opciones de configuración de la aplicación.</param>
    public JwtGenerator(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Genera y firma un token JWT con el identificador, correo y rol del usuario.
    /// </summary>
    /// <param name="usuario">Entidad con los datos del usuario autenticado.</param>
    /// <returns>Token JWT en formato string.</returns>
    public string GenerarToken(Usuario usuario)
    {
        string? secret = _configuration["JwtSettings:Secret"];
        string? issuer = _configuration["JwtSettings:Issuer"];
        string? audience = _configuration["JwtSettings:Audience"];
        string? expirationMinutesStr = _configuration["JwtSettings:ExpirationInMinutes"];

        if (string.IsNullOrWhiteSpace(secret))
            throw new InvalidOperationException("La clave secreta de JWT no está configurada.");

        double expirationMinutes = double.TryParse(expirationMinutesStr, out double minutes) ? minutes : 60;

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Name, usuario.NombreCompleto),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString())
        };

        JwtSecurityToken tokenDescriptor = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}