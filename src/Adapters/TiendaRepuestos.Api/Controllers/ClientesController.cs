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
/// Endpoints RESTful para la gestión, registro y consulta de clientes comerciales.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _clienteService;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ClientesController"/>.
    /// </summary>
    /// <param name="clienteService">Servicio de aplicación para operaciones de clientes.</param>
    public ClientesController(ClienteService clienteService)
    {
        _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
    }

    /// <summary>
    /// Registra un nuevo cliente comercial en el directorio de la tienda.
    /// </summary>
    /// <param name="dto">Datos del cliente a registrar.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>El cliente registrado con código 201 Created.</returns>
    [HttpPost]
    [Authorize(Roles = Roles.Todos)]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ClienteResponseDto>> RegistrarCliente(
        [FromBody] CrearClienteDto dto,
        CancellationToken cancellationToken)
    {
        ClienteResponseDto cliente = await _clienteService.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorDocumento), new { documento = cliente.Documento }, cliente);
    }

    /// <summary>
    /// Busca clientes que coincidan parcialmente por documento (Cédula/NIT) o nombre.
    /// </summary>
    /// <param name="termino">Término de búsqueda.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Lista de clientes coincidentes. Retorna 200 OK con lista vacía si no hay resultados.</returns>
    [HttpGet("buscar")]
    [Authorize(Roles = Roles.Todos)]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> Buscar(
        [FromQuery] string termino,
        CancellationToken cancellationToken)
    {
        IEnumerable<ClienteResponseDto> clientes = await _clienteService.BuscarAsync(termino, cancellationToken);
        return Ok(clientes);
    }

    /// <summary>
    /// Obtiene un cliente por su número exacto de documento.
    /// </summary>
    /// <param name="documento">Cédula o NIT.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Datos del cliente si existe; de lo contrario, 404.</returns>
    [HttpGet("por-documento/{documento}")]
    [Authorize(Roles = Roles.Todos)]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ClienteResponseDto>> ObtenerPorDocumento(
        [FromRoute] string documento,
        CancellationToken cancellationToken)
    {
        ClienteResponseDto? cliente = await _clienteService.ObtenerPorDocumentoAsync(documento, cancellationToken);
        if (cliente is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Cliente no encontrado",
                Detail = $"No existe ningún cliente registrado con el documento '{documento}'.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(cliente);
    }
}