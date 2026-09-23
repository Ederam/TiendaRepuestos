namespace TiendaRepuestos.Application.DTOs;

/// <summary>
/// DTO de entrada para incrementar o decrementar el stock de un repuesto.
/// </summary>
public class AjustarStockDto
{
    /// <summary>
    /// Cantidad a ajustar. Valor positivo para incrementos, valor negativo para salidas.
    /// </summary>
    public int Cantidad { get; set; }
}