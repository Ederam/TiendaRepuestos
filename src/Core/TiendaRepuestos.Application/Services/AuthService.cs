namespace TiendaRepuestos.Application.Services;

using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs.Auth;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Ports;

/// <summary>
/// Servicio del caso de uso encargado de la autenticación, verificación de credenciales y registro de usuarios.
/// </summary>
public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<RegistroUsuarioDto> _registroValidator;
    private readonly ILogger<AuthService> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AuthService"/>.
    /// </summary>
    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtGenerator jwtGenerator,
        IValidator<LoginDto> loginValidator,
        IValidator<RegistroUsuarioDto> registroValidator,
        ILogger<AuthService> logger)
    {
        _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _jwtGenerator = jwtGenerator ?? throw new ArgumentNullException(nameof(jwtGenerator));
        _loginValidator = loginValidator ?? throw new ArgumentNullException(nameof(loginValidator));
        _registroValidator = registroValidator ?? throw new ArgumentNullException(nameof(registroValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Autentica a un usuario y genera un token JWT si las credenciales son válidas.
    /// </summary>
    /// <param name="dto">Credenciales de inicio de sesión.</param>
    /// <param name="cancellationToken">Token de cancelación de la tarea.</param>
    /// <returns>Los datos del usuario autenticado junto al token de acceso.</returns>
    /// <exception cref="ValidationException">Se lanza si el DTO incumple las reglas de validación.</exception>
    /// <exception cref="UnauthorizedAccessException">Se lanza si el correo no existe o la contraseña es errónea.</exception>
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        await _loginValidator.ValidateAndThrowAsync(dto, cancellationToken);

        Usuario? usuario = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email, cancellationToken);

        if (usuario == null || !_passwordHasher.VerifyPassword(dto.Password, usuario.PasswordHash))
        {
            _logger.LogWarning("Intento fallido de inicio de sesión para el correo: {Email}", dto.Email);
            throw new UnauthorizedAccessException("Credenciales de acceso inválidas.");
        }

        if (!usuario.Activo)
        {
            _logger.LogWarning("Intento de acceso denegado para usuario inactivo: {Email}", dto.Email);
            throw new UnauthorizedAccessException("La cuenta de usuario se encuentra inactiva.");
        }

        string token = _jwtGenerator.GenerarToken(usuario);

        _logger.LogInformation("Usuario autenticado exitosamente: {Email} con Rol: {Rol}", usuario.Email, usuario.Rol);

        return AuthResponseDto.Create(usuario, token);
    }

    /// <summary>
    /// Registra un nuevo usuario con contraseña cifrada en la base de datos.
    /// </summary>
    /// <param name="dto">Datos para el registro del usuario.</param>
    /// <param name="cancellationToken">Token de cancelación de la tarea.</param>
    /// <returns>Los datos del usuario registrado y su token inicial.</returns>
    /// <exception cref="ValidationException">Se lanza si los datos son inválidos.</exception>
    /// <exception cref="InvalidOperationException">Se lanza si el email ya se encuentra en uso.</exception>
    public async Task<AuthResponseDto> RegistrarAsync(RegistroUsuarioDto dto, CancellationToken cancellationToken = default)
    {
        await _registroValidator.ValidateAndThrowAsync(dto, cancellationToken);

        Usuario? usuarioExistente = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email, cancellationToken);
        if (usuarioExistente != null)
        {
            _logger.LogWarning("Intento de registro duplicado con el correo: {Email}", dto.Email);
            throw new InvalidOperationException($"El correo '{dto.Email}' ya se encuentra registrado en el sistema.");
        }

        string passwordHash = _passwordHasher.HashPassword(dto.Password);
        Usuario nuevoUsuario = new Usuario(dto.NombreCompleto, dto.Email, passwordHash, dto.Rol);

        await _usuarioRepository.AddAsync(nuevoUsuario, cancellationToken);

        string token = _jwtGenerator.GenerarToken(nuevoUsuario);

        _logger.LogInformation("Nuevo usuario registrado: {Email} con ID: {UsuarioId}", nuevoUsuario.Email, nuevoUsuario.Id);

        return AuthResponseDto.Create(nuevoUsuario, token);
    }
}