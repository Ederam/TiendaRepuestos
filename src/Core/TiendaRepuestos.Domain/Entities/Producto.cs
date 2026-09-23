namespace TiendaRepuestos.Domain.Entities;

using System;
using TiendaRepuestos.Domain.Enums;

/// <summary>
/// Entidad principal que representa un repuesto o autoparte en el inventario.
/// </summary>
public class Producto
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string CodigoParte { get; private set; } = string.Empty;
    public decimal PrecioVenta { get; private set; }
    public int StockActual { get; private set; }
    public int StockMinimo { get; private set; }
    public EstadoProducto Estado { get; private set; }
    public Guid CategoriaId { get; private set; }
    public bool Activo { get; private set; }

    /// <summary>
    /// Constructor para Entity Framework Core.
    /// </summary>
    private Producto() { }

    /// <summary>
    /// Inicializa una nueva instancia de la entidad <see cref="Producto"/>.
    /// </summary>
    public Producto(
        string nombre,
        string codigoParte,
        decimal precioVenta,
        int stockInicial,
        int stockMinimo,
        EstadoProducto estado,
        Guid categoriaId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del repuesto no puede estar vacío.", nameof(nombre));
        }

        if (precioVenta <= 0)
        {
            throw new ArgumentException("El precio de venta debe ser mayor a cero.", nameof(precioVenta));
        }

        if (stockInicial < 0)
        {
            throw new ArgumentException("El stock inicial no puede ser negativo.", nameof(stockInicial));
        }

        if (stockMinimo < 0)
        {
            throw new ArgumentException("El stock mínimo no puede ser negativo.", nameof(stockMinimo));
        }

        Id = Guid.NewGuid();
        Nombre = nombre.Trim();
        CodigoParte = codigoParte?.Trim() ?? string.Empty;
        PrecioVenta = precioVenta;
        StockActual = stockInicial;
        StockMinimo = stockMinimo;
        Estado = estado;
        CategoriaId = categoriaId;
        Activo = true;
    }

    /// <summary>
    /// Actualiza la información básica del repuesto.
    /// </summary>
    public void ActualizarInformacion(string nombre, string codigoParte, Guid categoriaId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del repuesto no puede estar vacío.", nameof(nombre));
        }

        Nombre = nombre.Trim();
        CodigoParte = codigoParte?.Trim() ?? string.Empty;
        if (categoriaId != Guid.Empty)
        {
            CategoriaId = categoriaId;
        }
    }

    /// <summary>
    /// Actualiza el precio de venta del repuesto validando el rango permitido.
    /// </summary>
    public void ActualizarPrecio(decimal nuevoPrecio)
    {
        if (nuevoPrecio <= 0)
        {
            throw new ArgumentException("El precio de venta debe ser mayor a cero.", nameof(nuevoPrecio));
        }

        PrecioVenta = nuevoPrecio;
    }

    /// <summary>
    /// Ajusta el stock disponible del repuesto (incremento o decremento).
    /// </summary>
    public void AjustarStock(int cantidad)
    {
        int nuevoStock = StockActual + cantidad;
        if (nuevoStock < 0)
        {
            throw new InvalidOperationException($"No hay suficiente stock disponible. Stock actual: {StockActual}, ajuste intentado: {cantidad}.");
        }

        StockActual = nuevoStock;
    }

    /// <summary>
    /// Desactiva lógicamente el repuesto del catálogo.
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
    }
}