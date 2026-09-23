namespace TiendaRepuestos.Application.DTOs;

using System;
using TiendaRepuestos.Domain.Enums;

/// <summary>
/// DTO de entrada para la creación de un nuevo repuesto en el inventario.
/// </summary>
public record class CrearProductoDto(
    string Nombre,
    string? CodigoParte,
    decimal PrecioVenta,
    int StockInicial,
    int StockMinimo,
    EstadoProducto Estado,
    Guid CategoriaId
);