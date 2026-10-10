namespace TiendaRepuestos.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs.Reportes;
using TiendaRepuestos.Application.Ports;
using TiendaRepuestos.Domain.Constants;

/// <summary>
/// Endpoints REST para la extracción de métricas analíticas, auditoría de stock y reportes de facturación.
/// Restringido exclusivamente al perfil Administrador.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Administrador)]
public class ReportesController : ControllerBase
{
    private readonly IReportesRepository _reportesRepository;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ReportesController"/>.
    /// </summary>
    /// <param name="reportesRepository">Puerto de acceso a las consultas optimizadas con Dapper.</param>
    public ReportesController(IReportesRepository reportesRepository)
    {
        _reportesRepository = reportesRepository ?? throw new ArgumentNullException(nameof(reportesRepository));
    }

    /// <summary>
    /// Obtiene el listado de repuestos con existencias iguales o inferiores al nivel de stock mínimo.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Colección de productos con advertencia de reabastecimiento.</returns>
    [HttpGet("stock-critico")]
    [ProducesResponseType(typeof(IEnumerable<ReporteStockCriticoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObtenerStockCritico(CancellationToken cancellationToken)
    {
        IEnumerable<ReporteStockCriticoDto> resultado = await _reportesRepository.ObtenerProductosStockCriticoAsync(cancellationToken);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtiene el ranking de los repuestos con mayor demanda comercial.
    /// </summary>
    /// <param name="limite">Cantidad máxima de repuestos en el ranking (por defecto 5).</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Ranking de productos ordenado por volumen de unidades vendidas.</returns>
    [HttpGet("top-vendidos")]
    [ProducesResponseType(typeof(IEnumerable<ReporteTopProductoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObtenerTopVendidos([FromQuery] int limite = 5, CancellationToken cancellationToken = default)
    {
        IEnumerable<ReporteTopProductoDto> resultado = await _reportesRepository.ObtenerTopProductosMasVendidosAsync(limite, cancellationToken);
        return Ok(resultado);
    }
}