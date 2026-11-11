namespace TiendaRepuestos.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs.Auth;
using TiendaRepuestos.Application.Services;
using TiendaRepuestos.Domain.Constants;

/// <summary>
/// Controlador REST para la consulta del directorio de usuarios y vendedores activos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly AuthService _authService;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="UsuariosController"/>.
    /// </summary>
    public UsuariosController(AuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    /// <summary>
    /// Retorna la lista de usuarios activos (vendedores y administradores) para poblar el filtro de auditoría de ventas.
    /// </summary>
    [HttpGet("vendedores")]
    [Authorize(Roles = Roles.Todos)]
    [ProducesResponseType(typeof(IEnumerable<VendedorResumenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<VendedorResumenDto>>> ObtenerVendedores(CancellationToken cancellationToken)
    {
        IReadOnlyList<VendedorResumenDto> vendedores = await _authService.ObtenerVendedoresAsync(cancellationToken);
        return Ok(vendedores);
    }
}

