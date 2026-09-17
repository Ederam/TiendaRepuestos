namespace TiendaRepuestos.Domain.Entities;

using TiendaRepuestos.Domain.Enums;

public class Producto
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string CodigoParte { get; private set; } = string.Empty; // Referencia/OEM
    public decimal PrecioVenta { get; private set; }
    public int StockActual { get; private set; }
    public int StockMinimo { get; private set; }
    public EstadoProducto Estado { get; private set; }
    public Guid CategoriaId { get; private set; }
    public bool Activo { get; private set; }

    // Constructor privado requerido por Entity Framework Core
    private Producto() { }

    // Constructor de dominio con validación defensiva
    public Producto(string nombre, string codigoParte, decimal precioVenta, int stockInicial, EstadoProducto estado, Guid categoriaId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del repuesto no puede estar vacío.");

        if (precioVenta <= 0)
            throw new ArgumentException("El precio de venta debe ser mayor a cero.");

        if (stockInicial < 0)
            throw new ArgumentException("El stock inicial no puede ser negativo.");

        Id = Guid.NewGuid();
        Nombre = nombre.Trim();
        CodigoParte = codigoParte?.Trim().ToUpper() ?? string.Empty;
        PrecioVenta = precioVenta;
        StockActual = stockInicial;
        StockMinimo = 2; // Control de alerta base para repuestos
        Estado = estado;
        CategoriaId = categoriaId;
        Activo = true;
    }

    // Comportamiento del Dominio (Encapsulamiento)
    public void ReducirStock(int cantidad)
    {
        if (cantidad <= 0)
            throw new InvalidOperationException("La cantidad a descontar debe ser mayor a cero.");

        if (StockActual - cantidad < 0)
            throw new InvalidOperationException($"Stock insuficiente para '{Nombre}'. Disponible: {StockActual}.");

        StockActual -= cantidad;
    }

    public void AumentarStock(int cantidad)
    {
        if (cantidad <= 0)
            throw new InvalidOperationException("La cantidad a ingresar debe ser mayor a cero.");

        StockActual += cantidad;
    }
}