namespace TiendaRepuestos.Domain.Ports;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Contrato de abstracción de dominio para la generación atómica y segura de folios consecutivos de comprobantes.
/// Garantiza la ausencia de condiciones de carrera (race conditions) en entornos concurrentes.
/// </summary>
public interface IConsecutivoService
{
    /// <summary>
    /// Obtiene el siguiente número de comprobante secuencial formateado para ventas (ejemplo: VTA-00001).
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Identificador consecutivo formateado en cadena de texto.</returns>
    Task<string> ObtenerSiguienteConsecutivoVentaAsync(CancellationToken cancellationToken = default);
}