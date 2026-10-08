namespace TiendaRepuestos.Application.Ports;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs.Reportes;

/// <summary>
/// Puerto secundario para consultas analíticas de lectura de alto rendimiento (CQRS / Dapper).
/// </summary>
public interface IReportesRepository
{
    /// <summary>
    /// Consulta los repuestos cuyo inventario actual sea menor o igual al stock mínimo configurado.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Lista de repuestos en umbral crítico.</returns>
    Task<IEnumerable<ReporteStockCriticoDto>> ObtenerProductosStockCriticoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el ranking de los productos con mayor volumen de unidades vendidas dentro de un rango opcional.
    /// </summary>
    /// <param name="limite">Cantidad máxima de registros a retornar en el ranking.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Listado ordenado de mayor a menor según unidades comercializadas.</returns>
    Task<IEnumerable<ReporteTopProductoDto>> ObtenerTopProductosMasVendidosAsync(int limite = 5, CancellationToken cancellationToken = default);
}