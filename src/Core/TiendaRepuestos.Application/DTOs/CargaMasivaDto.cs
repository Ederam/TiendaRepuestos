namespace TiendaRepuestos.Application.DTOs;

using System;
using System.Collections.Generic;

/// <summary>
/// Representa el elemento individual recibido para una carga masiva de repuestos.
/// </summary>
public record class CargaMasivaItemDto(
    string CodigoParte,
    string Nombre,
    string? Descripcion,
    decimal PrecioVenta,
    decimal Costo,
    int StockActual,
    string? Ubicacion,
    string? Origen,
    string? MarcaRepuesto,
    string? MarcaVehiculo,
    string? Condicion,
    Guid? CategoriaId
);

/// <summary>
/// Resultado cuantitativo de la ejecución de una carga masiva.
/// </summary>
public record class CargaMasivaResultadoDto(
    int TotalProcesados,
    int Insertados,
    int Actualizados,
    int SinCambios,
    int Omitidos,
    IReadOnlyCollection<string> Mensajes
);