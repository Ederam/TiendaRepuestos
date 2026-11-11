namespace TiendaRepuestos.Application.DTOs.Auth;

using System;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// DTO liviano para poblar el selector de filtro de vendedores en el historial de ventas.
/// </summary>
/// <param name="Id">Identificador único (GUID) del usuario.</param>
/// <param name="NombreCompleto">Nombre completo del vendedor o administrador.</param>
/// <param name="Email">Correo electrónico institucional.</param>
/// <param name="Rol">Rol asignado (Vendedor o Administrador).</param>
public sealed record VendedorResumenDto(
    Guid Id,
    string NombreCompleto,
    string Email,
    string Rol)
{
    /// <summary>
    /// Proyecta una entidad <see cref="Usuario"/> a <see cref="VendedorResumenDto"/>.
    /// </summary>
    public static VendedorResumenDto FromEntity(Usuario usuario) =>
        new(usuario.Id, usuario.NombreCompleto, usuario.Email, usuario.Rol.ToString());
}

