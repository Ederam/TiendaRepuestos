namespace TiendaRepuestos.Application.DTOs;

using TiendaRepuestos.Domain.Entities;

/// <summary>
/// DTO de respuesta con la información detallada del repuesto.
/// </summary>
public record class ProductoResponseDto(
    Guid Id,
    string Nombre,
    string CodigoParte,
    decimal PrecioVenta,
    int StockActual,
    string Estado,
    bool Activo,
    decimal Costo = 0,
    int StockMinimo = 0,
    string Ubicacion = "",
    string Descripcion = "",
    Guid CategoriaId = default,
    string MarcaRepuesto = "",
    string MarcaVehiculo = "",
    string Condicion = "",
    string Origen = ""
)
{
    /// <summary>
    /// Mapea una entidad de dominio <see cref="Producto"/> a <see cref="ProductoResponseDto"/>.
    /// </summary>
    public static ProductoResponseDto FromEntity(Producto p) =>
        new(p.Id, p.Nombre, p.CodigoParte, p.PrecioVenta, p.StockActual, p.Estado.ToString(), p.Activo,
            p.Costo, p.StockMinimo, p.Ubicacion, p.Descripcion, p.CategoriaId,
            p.MarcaRepuesto, p.MarcaVehiculo, p.Condicion, p.Origen);
}