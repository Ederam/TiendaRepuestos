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
/// Conjunto de pruebas unitarias aisladas para verificar el comportamiento, reglas de negocio
/// y deducción de existencias en <see cref="VentaService"/>.
/// </summary>
public class VentaServiceTests
{
    private readonly Mock<IVentaRepository> _ventaRepositoryMock;
    private readonly Mock<IProductoRepository> _productoRepositoryMock;
    private readonly Mock<IConsecutivoService> _consecutivoServiceMock;
    private readonly Mock<ILogger<VentaService>> _loggerMock;
    private readonly IValidator<CrearVentaDto> _validator;
    private readonly VentaService _sut; // System Under Test

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VentaServiceTests"/> configurando dependencias simuladas.
    /// </summary>
    public VentaServiceTests()
    {
        _ventaRepositoryMock = new Mock<IVentaRepository>();
        _productoRepositoryMock = new Mock<IProductoRepository>();
        _consecutivoServiceMock = new Mock<IConsecutivoService>();
        _loggerMock = new Mock<ILogger<VentaService>>();

        // Configuración por defecto de la secuencia simulada
        _consecutivoServiceMock
            .Setup(c => c.ObtenerSiguienteConsecutivoVentaAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("VTA-00001");

        // Validador real para verificar reglas declarativas de entrada
        _validator = new CrearVentaDtoValidator();

        _sut = new VentaService(
            _ventaRepositoryMock.Object,
            _productoRepositoryMock.Object,
            _consecutivoServiceMock.Object,
            _validator,
            _loggerMock.Object);
    }

    /// <summary>
    /// Valida que con datos íntegros y existencias suficientes se procese la orden,
    /// se genere el folio oficial consecutivo y se reduzca el inventario.
    /// </summary>
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
            MetodoPago = MetodoPago.Efectivo,
            ClienteDocumento = "1020304050",
            ClienteNombre = "Taller Los Amigos",
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
        resultado.NumeroComprobante.Should().Be("VTA-00001");
        resultado.ClienteDocumento.Should().Be("1020304050");
        resultado.ClienteNombre.Should().Be("Taller Los Amigos");
        productoExistente.StockActual.Should().Be(7); // 10 - 3 = 7

        _productoRepositoryMock.Verify(r => r.UpdateAsync(productoExistente, It.IsAny<CancellationToken>()), Times.Once);
        _ventaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Venta>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Valida que cuando la venta es de mostrador (sin datos de cliente),
    /// se asignen los valores fiscales por defecto de Consumidor Final.
    /// </summary>
    [Fact]
    public async Task CrearVentaAsync_ConConsumidorFinal_DebeAsignarValoresPorDefecto()
    {
        // Arrange
        Guid productoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();
        Producto productoExistente = new Producto(
            "Bomba de Agua",
            "BA-001",
            120000m,
            5,
            1,
            EstadoProducto.Nuevo,
            categoriaId);

        CrearVentaDto dto = new CrearVentaDto
        {
            MetodoPago = MetodoPago.Transferencia,
            ClienteDocumento = null,
            ClienteNombre = null,
            Detalles = new List<CrearDetalleVentaDto>
            {
                new CrearDetalleVentaDto { ProductoId = productoId, Cantidad = 1 }
            }
        };

        _productoRepositoryMock
            .Setup(r => r.GetByIdAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(productoExistente);

        // Act
        VentaResponseDto resultado = await _sut.CrearVentaAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.ClienteDocumento.Should().Be("222222222222");
        resultado.ClienteNombre.Should().Be("Consumidor Final");
    }

    /// <summary>
    /// Valida que si la lista de detalles está vacía, el validador lance ValidationException.
    /// </summary>
    [Fact]
    public async Task CrearVentaAsync_SinDetalles_DebeLanzarValidationException()
    {
        // Arrange
        CrearVentaDto dto = new CrearVentaDto
        {
            MetodoPago = MetodoPago.Efectivo,
            Detalles = new List<CrearDetalleVentaDto>() // Lista vacía
        };

        // Act
        Func<Task> accion = async () => await _sut.CrearVentaAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<ValidationException>()
            .WithMessage("*al menos un producto*");

        _ventaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Venta>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Valida que si el repuesto especificado en la orden no existe en base de datos,
    /// la transacción se aborte lanzando KeyNotFoundException.
    /// </summary>
    [Fact]
    public async Task CrearVentaAsync_CuandoProductoNoExiste_DebeLanzarKeyNotFoundException()
    {
        // Arrange
        Guid productoInexistenteId = Guid.NewGuid();
        CrearVentaDto dto = new CrearVentaDto
        {
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

    /// <summary>
    /// Valida que cuando la cantidad solicitada excede la disponibilidad actual en stock,
    /// el servicio lance InvalidOperationException y no persista la venta.
    /// </summary>
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