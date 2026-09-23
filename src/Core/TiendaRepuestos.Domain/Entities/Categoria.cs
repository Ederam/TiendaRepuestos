namespace TiendaRepuestos.Domain.Entities;

using System;

/// <summary>
/// Representa una categoría o familia de repuestos en el sistema.
/// </summary>
public class Categoria
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool Activa { get; private set; }

    /// <summary>
    /// Constructor requerido por Entity Framework Core.
    /// </summary>
    private Categoria() { }

    /// <summary>
    /// Crea una nueva categoría validando reglas de negocio.
    /// </summary>
    public Categoria(string nombre, string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(nombre));
        }

        Id = Guid.NewGuid();
        Nombre = nombre.Trim();
        Description = descripcion?.Trim() ?? string.Empty;
        Activa = true;
    }

    /// <summary>
    /// Actualiza los datos informativos de la categoría.
    /// </summary>
    public void Actualizar(string nombre, string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(nombre));
        }

        Nombre = nombre.Trim();
        Description = descripcion?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Inactiva la categoría para impedir la asignación a nuevos repuestos.
    /// </summary>
    public void Desactivar()
    {
        Activa = false;
    }
}