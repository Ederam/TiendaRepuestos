namespace TiendaRepuestos.Application.Tests.Services;

using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Application.Services;
using TiendaRepuestos.Application.Validators;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Enums;
using TiendaRepuestos.Domain.Ports;
using Xunit;

/// <summary>
/// Conjunto de pruebas unitarias aisladas para verificar el comportamiento de <see cref="VentaService"/>.
/// </summary>
public class VentaServiceTests
{
    private readonly Mock<IVentaRepository> _ventaRepositoryMock;
    private readonly Mock<IProductoRepository> _productoRepositoryMock;
    private readonly Mock<ILogger<VentaService>> _loggerMock;
    private readonly IValidator<CrearVentaDto> _validator;
    private readonly VentaService _sut; // System Under Test (SUT)

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VentaServiceTests"/> configurando los mocks y el SUT.
    /// </summary>
    public VentaServiceTests()
    {
        _ventaRepositoryMock = new Mock<IVentaRepository>();
        _productoRepositoryMock = new Mock<IProductoRepository>();
        _loggerMock = new Mock<ILogger<VentaService>>();

        // Instancia real del validador para probar la integración con FluentValidation
        _validator = new CrearVentaDtoValidator();

        _sut = new VentaService(
            _ventaRepositoryMock.Object,
            _productoRepositoryMock.Object,
            _validator,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CrearVentaAsync_ConDatosValidosYStockSuficiente_DebeProcesarVentaYReducirStock()
    {
        // Arrange
        Guid productoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();
        Producto productoExistente = new Producto(
            "Pastillas de Freno",
            "PF-001",
            50.00m,
            10,
            2,
            EstadoProducto.Nuevo,
            categoriaId);

        CrearVentaDto dto = new CrearVentaDto
        {
            NumeroComprobante = "VTA-0001",
            MetodoPago = MetodoPago.Efectivo,
            Detalles = new List<CrearDetalleVentaDto>
            {
                new CrearDetalleVentaDto { ProductoId = productoId, Cantidad = 3 }
            }
        };

        _productoRepositoryMock
            .Setup(r => r.GetByIdAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(productoExistente);

        // Act
        VentaResponseDto resultado = await _sut.CrearVentaAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.NumeroComprobante.Should().Be("VTA-0001");
        productoExistente.StockActual.Should().Be(7); // 10 - 3 = 7

        _productoRepositoryMock.Verify(r => r.UpdateAsync(productoExistente, It.IsAny<CancellationToken>()), Times.Once);
        _ventaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Venta>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CrearVentaAsync_ConComprobanteInvalido_DebeLanzarValidationException()
    {
        // Arrange
        CrearVentaDto dto = new CrearVentaDto
        {
            NumeroComprobante = "INVALIDO-123", // Formato incorrecto
            MetodoPago = MetodoPago.Efectivo,
            Detalles = new List<CrearDetalleVentaDto>
            {
                new CrearDetalleVentaDto { ProductoId = Guid.NewGuid(), Cantidad = 1 }
            }
        };

        // Act
        Func<Task> accion = async () => await _sut.CrearVentaAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<ValidationException>()
            .WithMessage("*formato 'VTA-XXXX'*");

        _ventaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Venta>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CrearVentaAsync_CuandoProductoNoExiste_DebeLanzarKeyNotFoundException()
    {
        // Arrange
        Guid productoInexistenteId = Guid.NewGuid();
        CrearVentaDto dto = new CrearVentaDto
        {
            NumeroComprobante = "VTA-0002",
            MetodoPago = MetodoPago.Efectivo,
            Detalles = new List<CrearDetalleVentaDto>
            {
                new CrearDetalleVentaDto { ProductoId = productoInexistenteId, Cantidad = 1 }
            }
        };

        _productoRepositoryMock
            .Setup(r => r.GetByIdAsync(productoInexistenteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Producto?)null);

        // Act
        Func<Task> accion = async () => await _sut.CrearVentaAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{productoInexistenteId}*");
    }

    [Fact]
    public async Task CrearVentaAsync_CuandoStockEsInsuficiente_DebeLanzarInvalidOperationException()
    {
        // Arrange
        Guid productoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();
        Producto productoConPocoStock = new Producto(
            "Filtro Aceite",
            "FA-002",
            15.00m,
            2,
            1,
            EstadoProducto.Nuevo,
            categoriaId); // Solo 2 en stock

        CrearVentaDto dto = new CrearVentaDto
        {
            NumeroComprobante = "VTA-0003",
            MetodoPago = MetodoPago.TarjetaCredito,
            Detalles = new List<CrearDetalleVentaDto>
            {
                new CrearDetalleVentaDto { ProductoId = productoId, Cantidad = 5 } // Solicita 5
            }
        };

        _productoRepositoryMock
            .Setup(r => r.GetByIdAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(productoConPocoStock);

        // Act
        Func<Task> accion = async () => await _sut.CrearVentaAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Stock insuficiente*");

        _ventaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Venta>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}