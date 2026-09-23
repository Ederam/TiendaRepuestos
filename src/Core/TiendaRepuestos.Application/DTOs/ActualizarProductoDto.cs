namespace TiendaRepuestos.Application.DTOs;

using System;

/// <summary>
/// DTO de entrada para la actualización de un repuesto existente.
/// </summary>
public class ActualizarProductoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string CodigoParte { get; set; } = string.Empty;
    public decimal PrecioVenta { get; set; }
    public Guid CategoriaId { get; set; }
}