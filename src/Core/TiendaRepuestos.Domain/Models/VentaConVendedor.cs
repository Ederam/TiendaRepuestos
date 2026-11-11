namespace TiendaRepuestos.Domain.Models;

using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Modelo de lectura que combina el agregado <see cref="Venta"/> con el nombre del usuario que la emitió.
/// Se obtiene mediante JOIN con la tabla de usuarios; no es una entidad persistible.
/// </summary>
/// <param name="Venta">Agregado de venta con sus detalles.</param>
/// <param name="NombreVendedor">Nombre completo del vendedor; null si la venta es previa a la auditoría por usuario.</param>
public sealed record VentaConVendedor(Venta Venta, string? NombreVendedor);

