namespace TiendaRepuestos.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.Common;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Application.Services;
using TiendaRepuestos.Domain.Constants;

/// <summary>
/// Endpoints RESTful para la gestión del catálogo, consulta de repuestos y ajustes de inventario.
/// Aplica control de acceso basado en roles (RBAC).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere autenticación Bearer JWT en todas las operaciones del controlador
public class ProductosController : ControllerBase
{
    private readonly ProductoService _productoService;

    /// <summary>
    /// Inicializa una nueva instancia del controlador inyectando el servicio de aplicación correspondiente.
    /// </summary>
    /// <param name="productoService">Servicio de lógica de aplicación para la gestión de productos.</param>
    /// <exception cref="ArgumentNullException">Se lanza si el servicio inyectado es nulo.</exception>
    public ProductosController(ProductoService productoService)
    {
        _productoService = productoService ?? throw new ArgumentNullException(nameof(productoService));
    }

    /// <summary>
    /// Busca repuestos en el inventario por coincidencia en nombre o código de parte.
    /// </summary>
    /// <param name="q">Término opcional de búsqueda.</param>
    /// <param name="cancellationToken">Token para la cancelación cooperativa de la tarea.</param>
    /// <returns>Colección de repuestos que coinciden con el criterio de búsqueda.</returns>
    [HttpGet]
    [Authorize(Roles = Roles.Todos)]
    [ProducesResponseType(typeof(IEnumerable<ProductoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> Search(
        [FromQuery] string? q,
        CancellationToken cancellationToken)
    {
        IEnumerable<ProductoResponseDto> resultados = await _productoService.BuscarAsync(q, cancellationToken);
        return Ok(resultados);
    }

    /// <summary>
    /// Obtiene el detalle exhaustivo de un repuesto mediante su identificador único global (GUID).
    /// </summary>
    /// <param name="id">Identificador único del repuesto.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Datos del repuesto si existe en el catálogo.</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = Roles.Todos)]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProductoResponseDto>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        ProductoResponseDto? producto = await _productoService.ObtenerPorIdAsync(id, cancellationToken);
        if (producto is null)
        {
            throw new KeyNotFoundException($"El repuesto con ID '{id}' no existe en el catálogo.");
        }
        return Ok(producto);
    }

    /// <summary>
    /// Registra un nuevo repuesto en el inventario validando stock y categoría asociada.
    /// Restringido exclusivamente al perfil Administrador.
    /// </summary>
    /// <param name="dto">Estructura de datos para la creación del repuesto.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>El repuesto registrado con su identificador generado y código de estado 201 Created.</returns>
    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductoResponseDto>> Create(
        [FromBody] CrearProductoDto dto,
        CancellationToken cancellationToken)
    {
        ProductoResponseDto productoCreado = await _productoService.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = productoCreado.Id }, productoCreado);
    }

    /// <summary>
    /// Actualiza la información técnica, descripción y precio de venta de un repuesto existente.
    /// Restringido exclusivamente al perfil Administrador.
    /// </summary>
    /// <param name="id">Identificador único del repuesto a modificar.</param>
    /// <param name="dto">Información actualizada del repuesto.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Datos actualizados del repuesto.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductoResponseDto>> Update(
        [FromRoute] Guid id,
        [FromBody] ActualizarProductoDto dto,
        CancellationToken cancellationToken)
    {
        // Se delega el control de excepciones (KeyNotFoundException, etc.) al middleware RFC 7807
        ProductoResponseDto productoActualizado = await _productoService.ActualizarAsync(id, dto, cancellationToken);
        return Ok(productoActualizado);
    }

    /// <summary>
    /// Realiza un ajuste manual de existencias físicas sobre el inventario de un repuesto.
    /// Restringido exclusivamente al perfil Administrador.
    /// </summary>
    /// <param name="id">Identificador único del repuesto a ajustar.</param>
    /// <param name="dto">Magnitud positiva o negativa del ajuste de existencias.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Datos del repuesto con el saldo de stock actualizado.</returns>
    [HttpPatch("{id:guid}/stock")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductoResponseDto>> AjustarStock(
        [FromRoute] Guid id,
        [FromBody] AjustarStockDto dto,
        CancellationToken cancellationToken)
    {
        // Se eliminan bloques try-catch manuales para mantener el controlador limpio y homogéneo con RFC 7807
        ProductoResponseDto productoActualizado = await _productoService.AjustarStockAsync(id, dto.Cantidad, cancellationToken);
        return Ok(productoActualizado);
    }

    /// <summary>
    /// Consulta el catálogo de repuestos de forma paginada con filtros por nombre, código y categoría.
    /// </summary>
    /// <param name="parameters">Parámetros de navegación y criterios de filtro.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Página de repuestos con metadatos de paginación.</returns>
    [HttpGet("paginado")]
    [Authorize(Roles = Roles.Todos)]
    [ProducesResponseType(typeof(PagedResult<ProductoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<ProductoResponseDto>>> ObtenerPaginado(
        [FromQuery] ConsultaProductosParameters parameters,
        CancellationToken cancellationToken)
    {
        PagedResult<ProductoResponseDto> resultado = await _productoService.ObtenerPaginadoAsync(parameters, cancellationToken);
        return Ok(resultado);
    }
}