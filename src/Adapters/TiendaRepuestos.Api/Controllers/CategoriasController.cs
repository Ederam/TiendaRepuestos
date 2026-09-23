namespace TiendaRepuestos.Api.Controllers;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Application.Services;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly CategoriaService _categoriaService;

    public CategoriasController(CategoriaService categoriaService)
    {
        _categoriaService = categoriaService ?? throw new ArgumentNullException(nameof(categoriaService));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        IEnumerable<CategoriaResponseDto> categorias = await _categoriaService.ObtenerTodasAsync(cancellationToken);
        return Ok(categorias);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        CategoriaResponseDto? categoria = await _categoriaService.ObtenerPorIdAsync(id, cancellationToken);
        if (categoria is null)
        {
            return NotFound(new { mensaje = $"No se encontró la categoría con ID: {id}" });
        }

        return Ok(categoria);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CrearCategoriaDto dto, CancellationToken cancellationToken)
    {
        CategoriaResponseDto nuevaCategoria = await _categoriaService.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = nuevaCategoria.Id }, nuevaCategoria);
    }
}