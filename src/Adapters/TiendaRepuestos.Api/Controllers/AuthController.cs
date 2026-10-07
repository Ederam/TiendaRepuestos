namespace TiendaRepuestos.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs.Auth;
using TiendaRepuestos.Application.Services;

/// <summary>
/// Controlador expuesto para la gestión de credenciales, registro y autenticación mediante JWT.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AuthController"/>.
    /// </summary>
    /// <param name="authService">Servicio de aplicación para autenticación.</param>
    public AuthController(AuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    /// <summary>
    /// Registra un nuevo usuario con contraseña cifrada en el sistema.
    /// </summary>
    /// <param name="dto">Datos del usuario a registrar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Credenciales y token de sesión generado.</returns>
    [HttpPost("registro")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistroUsuarioDto dto,
        CancellationToken cancellationToken)
    {
        AuthResponseDto resultado = await _authService.RegistrarAsync(dto, cancellationToken);
        return Ok(resultado);
    }

    /// <summary>
    /// Valida credenciales e inicia sesión generando un token JWT firmado.
    /// </summary>
    /// <param name="dto">Credenciales de inicio de sesión.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Token de acceso e información del usuario autenticado.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto dto,
        CancellationToken cancellationToken)
    {
        AuthResponseDto resultado = await _authService.LoginAsync(dto, cancellationToken);
        return Ok(resultado);
    }
}