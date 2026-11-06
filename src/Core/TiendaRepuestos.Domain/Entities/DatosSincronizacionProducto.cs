namespace TiendaRepuestos.Domain.Entities;

/// <summary>
/// Parámetros encapsulados para la sincronización idempotente de un repuesto en cargas masivas.
/// </summary>
public record class DatosSincronizacionProducto(
    string Nombre,
    decimal PrecioVenta,
    decimal Costo,
    int NuevoStock,
    string? Ubicacion,
    string? Descripcion,
    string? MarcaRepuesto,
    string? MarcaVehiculo
);