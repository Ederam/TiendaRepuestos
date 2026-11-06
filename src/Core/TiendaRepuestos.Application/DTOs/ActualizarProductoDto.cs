namespace TiendaRepuestos.Application.DTOs;

using System;

/// <summary>
/// DTO de entrada para la actualización de un repuesto existente.
/// </summary>
public record class ActualizarProductoDto(
    string Nombre,
    string CodigoParte,
    decimal PrecioVenta,
    decimal Costo,
    Guid CategoriaId
);