namespace TiendaRepuestos.Application.DTOs;

using System;

/// <summary>
/// DTO de entrada para la actualización de un repuesto existente.
/// Permite actualizar precio, costo, categoría, stock mínimo y metadatos de mostrador.
/// </summary>
public record class ActualizarProductoDto(
    string Nombre,
    string CodigoParte,
    decimal PrecioVenta,
    decimal Costo,
    Guid CategoriaId,
    int StockActual = 0,
    int StockMinimo = 2,
    string? Ubicacion = null,
    string? MarcaRepuesto = null,
    string? MarcaVehiculo = null,
    string? Condicion = null,
    string? Descripcion = null,
    string? Origen = null
);
