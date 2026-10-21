namespace TiendaRepuestos.Application.DTOs;

using System;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Parámetros de entrada para la creación de un nuevo cliente comercial.
/// </summary>
public record class CrearClienteDto(
    string Documento,
    string Nombre,
    string? Telefono,
    string? Correo,
    string? Direccion,
    bool? EsFacturaElectronica
);

/// <summary>
/// Proyección de datos de cliente retornada hacia los adaptadores de presentación.
/// </summary>
public record class ClienteResponseDto(
    Guid Id,
    string Documento,
    string Nombre,
    string Telefono,
    string Correo,
    string Direccion,
    bool EsFacturaElectronica,
    bool Activo
)
{
    /// <summary>
    /// Mapea una entidad de dominio Cliente hacia su DTO de transporte de alta velocidad sin Reflection.
    /// </summary>
    public static ClienteResponseDto FromEntity(Cliente entidad)
    {
        return new ClienteResponseDto(
            entidad.Id,
            entidad.Documento,
            entidad.Nombre,
            entidad.Telefono,
            entidad.Correo,
            entidad.Direccion,
            entidad.EsFacturaElectronica,
            entidad.Activo
        );
    }
}