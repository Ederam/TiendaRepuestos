namespace TiendaRepuestos.Application.DTOs;

using System;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// DTO de entrada para crear una categoría.
/// </summary>
public class CrearCategoriaDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

/// <summary>
/// DTO de salida para presentar información de categorías.
/// </summary>
public class CategoriaResponseDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activa { get; set; }

    public static CategoriaResponseDto FromEntity(Categoria categoria)
    {
        return new CategoriaResponseDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Description,
            Activa = categoria.Activa
        };
    }
}