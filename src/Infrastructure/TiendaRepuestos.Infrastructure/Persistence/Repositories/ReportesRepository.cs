namespace TiendaRepuestos.Infrastructure.Persistence.Repositories;

using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs.Reportes;
using TiendaRepuestos.Application.Ports;

/// <summary>
/// Adaptador de infraestructura para reportes analíticos utilizando Dapper y consultas directas sobre PostgreSQL.
/// </summary>
public class ReportesRepository : IReportesRepository
{
    private readonly string _connectionString;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ReportesRepository"/>.
    /// </summary>
    /// <param name="configuration">Instancia para obtener la cadena de conexión configurada.</param>
    public ReportesRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ReporteStockCriticoDto>> ObtenerProductosStockCriticoAsync(CancellationToken cancellationToken = default)
    {
        // Se utilizan comillas dobles para respetar la convención PascalCase generada por EF Core en PostgreSQL
        const string sql = @"
            SELECT 
                p.""Id"" AS Id,
                p.""Nombre"" AS Nombre,
                p.""CodigoParte"" AS CodigoParte,
                COALESCE(c.""Nombre"", 'Sin Categoría') AS Categoria,
                p.""StockActual"" AS StockActual,
                p.""StockMinimo"" AS StockMinimo
            FROM ""productos"" p
            LEFT JOIN ""Categorias"" c ON p.""CategoriaId"" = c.""Id""
            WHERE p.""StockActual"" <= p.""StockMinimo""
              AND p.""Activo"" = TRUE
            ORDER BY (p.""StockActual"" - p.""StockMinimo"") ASC;";

        await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
        CommandDefinition command = new CommandDefinition(sql, cancellationToken: cancellationToken);

        return await connection.QueryAsync<ReporteStockCriticoDto>(command);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ReporteTopProductoDto>> ObtenerTopProductosMasVendidosAsync(int limite = 5, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                dv.""ProductoId"" AS ProductoId,
                dv.""NombreProducto"" AS NombreProducto,
                SUM(dv.""Cantidad"") AS TotalUnidadesVendidas,
                SUM(dv.""Subtotal"") AS TotalRecaudado
            FROM ""DetallesVenta"" dv
            INNER JOIN ""Ventas"" v ON dv.""VentaId"" = v.""Id""
            WHERE v.""Estado"" = 0 -- 0: Completada
            GROUP BY dv.""ProductoId"", dv.""NombreProducto""
            ORDER BY TotalUnidadesVendidas DESC
            LIMIT @Limite;";

        await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
        CommandDefinition command = new CommandDefinition(sql, new { Limite = limite }, cancellationToken: cancellationToken);

        return await connection.QueryAsync<ReporteTopProductoDto>(command);
    }
}