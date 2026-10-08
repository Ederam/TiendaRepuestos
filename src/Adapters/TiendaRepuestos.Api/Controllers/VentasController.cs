namespace TiendaRepuestos.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Application.Services;
using TiendaRepuestos.Domain.Constants;

/// <summary>
/// Endpoints REST para la emisión de comprobantes, facturación en caja y consulta de historial de ventas (POS).
/// Aplica control de acceso basado en roles (RBAC).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Exige autenticación Bearer JWT en todas las operaciones del controlador
public class VentasController : ControllerBase
{
    private readonly VentaService _ventaService;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VentasController"/> inyectando el servicio de aplicación de ventas.
    /// </summary>
    /// <param name="ventaService">Servicio de orquestación de operaciones de ventas.</param>
    /// <exception cref="ArgumentNullException">Se lanza si el servicio provisto es nulo.</exception>
    public VentasController(VentaService ventaService)
    {
        _ventaService = ventaService ?? throw new ArgumentNullException(nameof(ventaService));
    }

    /// <summary>
    /// Obtiene el historial completo de comprobantes de ventas procesadas en el sistema.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Colección con el resumen de comprobantes de ventas.</returns>
    [HttpGet]
    [Authorize(Roles = Roles.Todos)]
    [ProducesResponseType(typeof(IEnumerable<VentaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var ventas = await _ventaService.ObtenerTodasAsync(cancellationToken);
        return Ok(ventas);
    }

    /// <summary>
    /// Obtiene el detalle completo de un comprobante de venta a partir de su identificador único (GUID).
    /// </summary>
    /// <param name="id">Identificador único de la venta.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Comprobante de venta con el desglose de productos, cantidades y totales.</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = Roles.Todos)]
    [ProducesResponseType(typeof(VentaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var venta = await _ventaService.ObtenerPorIdAsync(id, cancellationToken);
        if (venta is null)
        {
            throw new KeyNotFoundException($"No se encontró la venta con ID: {id}");
        }

        return Ok(venta);
    }

    /// <summary>
    /// Procesa una venta en el punto de venta (POS), valida existencias, descuenta inventario y emite comprobante.
    /// Accesible tanto por cajeros (Vendedor) como por la gerencia (Administrador).
    /// </summary>
    /// <param name="dto">Estructura con los detalles, método de pago y comprobante a generar.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>El comprobante emitido con cálculos fiscales y código de estado 201 Created.</returns>
    [HttpPost]
    [Authorize(Roles = Roles.Todos)]
    [ProducesResponseType(typeof(VentaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(HttpValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CrearVentaDto dto, CancellationToken cancellationToken)
    {
        // Se delega el control de excepciones (stock insuficiente, comprobante inválido) al middleware RFC 7807
        var ventaCreada = await _ventaService.CrearVentaAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = ventaCreada.Id }, ventaCreada);
    }
}