namespace TiendaRepuestos.Application.Tests.Services;

using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs.Auth;
using TiendaRepuestos.Application.Services;
using TiendaRepuestos.Application.Validators;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Enums;
using TiendaRepuestos.Domain.Ports;
using Xunit;

/// <summary>
/// Conjunto de pruebas unitarias aisladas para el caso de uso <see cref="AuthService"/>.
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtGenerator> _jwtGeneratorMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<RegistroUsuarioDto> _registroValidator;
    private readonly AuthService _sut; // System Under Test

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AuthServiceTests"/> configurando mocks y validadores reales.
    /// </summary>
    public AuthServiceTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtGeneratorMock = new Mock<IJwtGenerator>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        // Usamos instancias reales de los validadores para evaluar la integración del pipeline de reglas
        _loginValidator = new LoginDtoValidator();
        _registroValidator = new RegistroUsuarioDtoValidator();

        _sut = new AuthService(
            _usuarioRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtGeneratorMock.Object,
            _loginValidator,
            _registroValidator,
            _loggerMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ConCredencialesValidas_DebeRetornarAuthResponseConToken()
    {
        // Arrange
        LoginDto dto = new LoginDto
        {
            Email = "camilo@repuestos.com",
            Password = "Password123*"
        };

        Usuario usuarioExistente = new Usuario(
            "Camilo Ramirez",
            dto.Email,
            "hash_seguro_almacenado",
            RolUsuario.Administrador);

        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuarioExistente);

        _passwordHasherMock
            .Setup(h => h.VerifyPassword(dto.Password, usuarioExistente.PasswordHash))
            .Returns(true);

        _jwtGeneratorMock
            .Setup(g => g.GenerarToken(usuarioExistente))
            .Returns("token_jwt_valido_simulado");

        // Act
        AuthResponseDto resultado = await _sut.LoginAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Email.Should().Be(dto.Email);
        resultado.Rol.Should().Be("Administrador");
        resultado.Token.Should().Be("token_jwt_valido_simulado");

        _jwtGeneratorMock.Verify(g => g.GenerarToken(usuarioExistente), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_CuandoPasswordEsIncorrecto_DebeLanzarUnauthorizedAccessException()
    {
        // Arrange
        LoginDto dto = new LoginDto
        {
            Email = "camilo@repuestos.com",
            Password = "PasswordErroneo"
        };

        Usuario usuarioExistente = new Usuario(
            "Camilo Ramirez",
            dto.Email,
            "hash_seguro_almacenado",
            RolUsuario.Administrador);

        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuarioExistente);

        _passwordHasherMock
            .Setup(h => h.VerifyPassword(dto.Password, usuarioExistente.PasswordHash))
            .Returns(false); // Contraseña no coincide

        // Act
        Func<Task> accion = async () => await _sut.LoginAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales de acceso inválidas.");

        _jwtGeneratorMock.Verify(g => g.GenerarToken(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_CuandoUsuarioNoExiste_DebeLanzarUnauthorizedAccessException()
    {
        // Arrange
        LoginDto dto = new LoginDto
        {
            Email = "inexistente@repuestos.com",
            Password = "Password123*"
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        // Act
        Func<Task> accion = async () => await _sut.LoginAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales de acceso inválidas.");
    }

    [Fact]
    public async Task LoginAsync_CuandoUsuarioEstaInactivo_DebeLanzarUnauthorizedAccessException()
    {
        // Arrange
        LoginDto dto = new LoginDto
        {
            Email = "inactivo@repuestos.com",
            Password = "Password123*"
        };

        Usuario usuarioInactivo = new Usuario(
            "Usuario Inactivo",
            dto.Email,
            "hash_valido",
            RolUsuario.Vendedor);

        usuarioInactivo.Desactivar(); // Se deshabilita la cuenta

        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuarioInactivo);

        _passwordHasherMock
            .Setup(h => h.VerifyPassword(dto.Password, usuarioInactivo.PasswordHash))
            .Returns(true);

        // Act
        Func<Task> accion = async () => await _sut.LoginAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("La cuenta de usuario se encuentra inactiva.");
    }

    [Fact]
    public async Task RegistrarAsync_ConDatosValidos_DebeCrearUsuarioYRetornarToken()
    {
        // Arrange
        RegistroUsuarioDto dto = new RegistroUsuarioDto
        {
            NombreCompleto = "Nuevo Empleado",
            Email = "empleado@repuestos.com",
            Password = "PasswordSeguro123*",
            Rol = RolUsuario.Vendedor
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null); // Correo disponible

        _passwordHasherMock
            .Setup(h => h.HashPassword(dto.Password))
            .Returns("hash_generado_bcrypt");

        _jwtGeneratorMock
            .Setup(g => g.GenerarToken(It.IsAny<Usuario>()))
            .Returns("token_jwt_inicial");

        // Act
        AuthResponseDto resultado = await _sut.RegistrarAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Email.Should().Be(dto.Email);
        resultado.Rol.Should().Be("Vendedor");
        resultado.Token.Should().Be("token_jwt_inicial");

        _usuarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_CuandoEmailYaExiste_DebeLanzarInvalidOperationException()
    {
        // Arrange
        RegistroUsuarioDto dto = new RegistroUsuarioDto
        {
            NombreCompleto = "Usuario Repetido",
            Email = "duplicado@repuestos.com",
            Password = "Password123*",
            Rol = RolUsuario.Vendedor
        };

        Usuario usuarioExistente = new Usuario(
            "Usuario Ya Registrado",
            dto.Email,
            "hash_existente",
            RolUsuario.Vendedor);

        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuarioExistente);

        // Act
        Func<Task> accion = async () => await _sut.RegistrarAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*{dto.Email}*");

        _usuarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarAsync_ConDatosInvalidos_DebeLanzarValidationException()
    {
        // Arrange (email mal formado y contraseña demasiado corta)
        RegistroUsuarioDto dto = new RegistroUsuarioDto
        {
            NombreCompleto = "",
            Email = "correo-invalido",
            Password = "123",
            Rol = RolUsuario.Vendedor
        };

        // Act
        Func<Task> accion = async () => await _sut.RegistrarAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<ValidationException>();

        _usuarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}