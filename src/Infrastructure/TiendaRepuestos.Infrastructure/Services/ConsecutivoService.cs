namespace TiendaRepuestos.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Domain.Ports;
using TiendaRepuestos.Infrastructure.Persistence;

/// <summary>
/// Adaptador de infraestructura que gestiona secuencias nativas en PostgreSQL para generar
/// consecutivos de facturación sin riesgo de colisiones entre cajeros concurrentes.
/// </summary>
public class ConsecutivoService : IConsecutivoService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ConsecutivoService"/>.
    /// </summary>
    /// <param name="context">Contexto de acceso a datos de Entity Framework Core.</param>
    public ConsecutivoService(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Invoca la secuencia nativa 'seq_ventas_consecutivo' en PostgreSQL y da formato 'VTA-XXXXX'.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Cadena con el folio oficial generado (ejemplo: VTA-00001).</returns>
    public async Task<string> ObtenerSiguienteConsecutivoVentaAsync(CancellationToken cancellationToken = default)
    {
        var connection = _context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        using var command = connection.CreateCommand();
        // Garantiza la creación de la secuencia si la BD es inicializada desde cero
        command.CommandText = @"
            CREATE SEQUENCE IF NOT EXISTS seq_ventas_consecutivo START WITH 1 INCREMENT BY 1;
            SELECT nextval('seq_ventas_consecutivo');";

        var result = await command.ExecuteScalarAsync(cancellationToken);
        long consecutivo = Convert.ToInt64(result);

        return $"VTA-{consecutivo:D5}";
    }
}