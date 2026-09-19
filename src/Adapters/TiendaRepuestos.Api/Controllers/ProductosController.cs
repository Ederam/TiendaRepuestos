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

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ProductosController"/>.
    /// </summary>
    /// <param name="productoService">Servicio de aplicación para productos.</param>
    public ProductosController(ProductoService productoService)
    {
        _productoService = productoService ?? throw new ArgumentNullException(nameof(productoService));
    }

    /// <summary>
    /// Busca repuestos por coincidencia en nombre o código de parte.
    /// </summary>
    /// <param name="q">Término de búsqueda opcional.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Colección de repuestos coincidentes.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> Search([FromQuery] string? q, CancellationToken cancellationToken)
    {
        IEnumerable<ProductoResponseDto> resultados = await _productoService.BuscarAsync(q, cancellationToken);
        return Ok(resultados);
    }

    /// <summary>
    /// Registra un nuevo repuesto en el inventario.
    /// </summary>
    /// <param name="dto">Datos para la creación del repuesto.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El repuesto registrado con su identificador generado.</returns>
    [HttpPost]
    public async Task<ActionResult<ProductoResponseDto>> Create([FromBody] CrearProductoDto dto, CancellationToken cancellationToken)
    {
        ProductoResponseDto productoCreado = await _productoService.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(Search), new { q = productoCreado.Nombre }, productoCreado);
    }
}