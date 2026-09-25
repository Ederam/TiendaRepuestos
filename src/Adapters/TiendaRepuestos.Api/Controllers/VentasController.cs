namespace TiendaRepuestos.Api.Controllers;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Application.Services;

/// <summary>
/// API REST para la gestión de ventas y emisión de comprobantes en el punto de venta (POS).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly VentaService _ventaService;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VentasController"/>.
    /// </summary>
    /// <param name="ventaService">Servicio de aplicación para las ventas.</param>
    public VentasController(VentaService ventaService)
    {
        _ventaService = ventaService ?? throw new ArgumentNullException(nameof(ventaService));
    }

    /// <summary>
    /// Obtiene el historial completo de ventas procesadas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de comprobantes de ventas.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VentaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var ventas = await _ventaService.ObtenerTodasAsync(cancellationToken);
        return Ok(ventas);
    }

    /// <summary>
    /// Obtiene el detalle de un comprobante de venta por su ID.
    /// </summary>
    /// <param name="id">Identificador único (GUID) de la venta.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Detalle de la venta si existe.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VentaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var venta = await _ventaService.ObtenerPorIdAsync(id, cancellationToken);
        if (venta is null)
        {
            return NotFound(new { mensaje = $"No se encontró la venta con ID: {id}" });
        }

        return Ok(venta);
    }

    /// <summary>
    /// Procesa una venta en la caja registradora, valida stock, descuenta inventario y emite comprobante.
    /// </summary>
    /// <param name="dto">Datos requeridos para procesar la venta.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El comprobante generado con subtotales e impuestos.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(VentaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CrearVentaDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var ventaCreada = await _ventaService.CrearVentaAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = ventaCreada.Id }, ventaCreada);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}