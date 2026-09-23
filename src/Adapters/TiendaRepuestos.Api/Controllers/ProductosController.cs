namespace TiendaRepuestos.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Application.Services;

/// <summary>
/// Endpoints para la gestión y consulta de repuestos del inventario.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly ProductoService _productoService;

    public ProductosController(ProductoService productoService)
    {
        _productoService = productoService ?? throw new ArgumentNullException(nameof(productoService));
    }

    /// <summary>
    /// Busca repuestos por coincidencia en nombre o código de parte.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> Search([FromQuery] string? q, CancellationToken cancellationToken)
    {
        IEnumerable<ProductoResponseDto> resultados = await _productoService.BuscarAsync(q, cancellationToken);
        return Ok(resultados);
    }

    /// <summary>
    /// Obtiene el detalle de un repuesto por su identificador único.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductoResponseDto>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        ProductoResponseDto? producto = await _productoService.ObtenerPorIdAsync(id, cancellationToken);
        if (producto is null)
        {
            return NotFound(new { mensaje = $"El repuesto con ID '{id}' no existe." });
        }
        return Ok(producto);
    }

    /// <summary>
    /// Registra un nuevo repuesto en el inventario.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductoResponseDto>> Create([FromBody] CrearProductoDto dto, CancellationToken cancellationToken)
    {
        ProductoResponseDto productoCreado = await _productoService.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = productoCreado.Id }, productoCreado);
    }

    /// <summary>
    /// Actualiza los datos generales y el precio de un repuesto existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductoResponseDto>> Update([FromRoute] Guid id, [FromBody] ActualizarProductoDto dto, CancellationToken cancellationToken)
    {
        try
        {
            ProductoResponseDto productoActualizado = await _productoService.ActualizarAsync(id, dto, cancellationToken);
            return Ok(productoActualizado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Ajusta el stock de un repuesto (incremento o decremento).
    /// </summary>
    [HttpPatch("{id:guid}/stock")]
    public async Task<ActionResult<ProductoResponseDto>> AjustarStock([FromRoute] Guid id, [FromBody] AjustarStockDto dto, CancellationToken cancellationToken)
    {
        try
        {
            ProductoResponseDto productoActualizado = await _productoService.AjustarStockAsync(id, dto.Cantidad, cancellationToken);
            return Ok(productoActualizado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}