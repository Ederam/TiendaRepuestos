namespace TiendaRepuestos.Application.DTOs;

using TiendaRepuestos.Domain.Enums;

/// <summary>
/// Objeto de transferencia de datos para el registro de un nuevo repuesto.
/// </summary>
/// <param name="Nombre">Nombre comercial o descripción del repuesto.</param>
/// <param name="CodigoParte">Código OEM o referencia del repuesto.</param>
/// <param name="PrecioVenta">Precio unitario de venta al público.</param>
/// <param name="StockInicial">Cantidad inicial disponible en inventario.</param>
/// <param name="Estado">Estado físico del repuesto (Nuevo o Segunda Mano).</param>
/// <param name="CategoriaId">Identificador de la categoría asociada.</param>
public record class CrearProductoDto(
    string Nombre,
    string? CodigoParte,
    decimal PrecioVenta,
    int StockInicial,
    EstadoProducto Estado,
    Guid CategoriaId
);