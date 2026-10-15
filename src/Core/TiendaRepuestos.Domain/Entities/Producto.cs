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
    public string Descripcion { get; private set; } = string.Empty;
    public decimal PrecioVenta { get; private set; }
    public decimal Costo { get; private set; }
    public int StockActual { get; private set; }
    public int StockMinimo { get; private set; }
    public EstadoProducto Estado { get; private set; }
    public string Origen { get; private set; } = "PROPIO_MACROLATAS";
    public string Ubicacion { get; private set; } = "BODEGA PRINCIPAL";
    public string MarcaRepuesto { get; private set; } = "GENÉRICO";
    public string MarcaVehiculo { get; private set; } = "MULTIMARCA";
    public string Condicion { get; private set; } = "Nuevo";
    public Guid CategoriaId { get; private set; }
    public bool Activo { get; private set; }

    /// <summary>
    /// Constructor protegido para el mapeo relacional de Entity Framework Core.
    /// </summary>
    private Producto() { }

    /// <summary>
    /// Inicializa una nueva instancia de repuesto con los atributos base requeridos.
    /// </summary>
    /// <param name="nombre">Nombre comercial o descripción corta del repuesto.</param>
    /// <param name="codigoParte">Código de referencia o parte técnica asignado por fabricante.</param>
    /// <param name="precioVenta">Precio final de venta al público en COP.</param>
    /// <param name="stockInicial">Existencias iniciales físicas en inventario.</param>
    /// <param name="stockMinimo">Nivel de inventario de alerta mínima para reposición.</param>
    /// <param name="estado">Estado inicial del ciclo de vida del repuesto.</param>
    /// <param name="categoriaId">Identificador único de la categoría asociada.</param>
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
        CodigoParte = codigoParte?.Trim().ToUpperInvariant() ?? string.Empty;
        PrecioVenta = precioVenta;
        Costo = 0m;
        StockActual = stockInicial;
        StockMinimo = stockMinimo;
        Estado = estado;
        CategoriaId = categoriaId;
        Origen = "PROPIO_MACROLATAS";
        Ubicacion = "BODEGA PRINCIPAL";
        MarcaRepuesto = "GENÉRICO";
        MarcaVehiculo = "MULTIMARCA";
        Condicion = "Nuevo";
        Descripcion = string.Empty;
        Activo = true;
    }

    /// <summary>
    /// Asigna o complementa los metadatos específicos de catálogo y procedencia de la autoparte.
    /// </summary>
    /// <param name="origen">Origen del inventario (propio o aliado externo).</param>
    /// <param name="ubicacion">Ubicación física en bodega o local comercial.</param>
    /// <param name="marcaRepuesto">Marca de la pieza de recambio.</param>
    /// <param name="marcaVehiculo">Marca del vehículo compatible.</param>
    /// <param name="condicion">Condición de la autoparte (Nuevo / Segunda).</param>
    /// <param name="costo">Costo de adquisición de la pieza.</param>
    /// <param name="descripcion">Descripción técnica o detalles adicionales.</param>
    public void AsignarMetadatosCatalogo(
        string? origen,
        string? ubicacion,
        string? marcaRepuesto,
        string? marcaVehiculo,
        string? condicion,
        decimal costo,
        string? descripcion)
    {
        if (!string.IsNullOrWhiteSpace(origen)) Origen = origen.Trim().ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(ubicacion)) Ubicacion = ubicacion.Trim().ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(marcaRepuesto)) MarcaRepuesto = marcaRepuesto.Trim();
        if (!string.IsNullOrWhiteSpace(marcaVehiculo)) MarcaVehiculo = marcaVehiculo.Trim();
        if (!string.IsNullOrWhiteSpace(condicion)) Condicion = condicion.Trim();
        if (costo >= 0) Costo = costo;
        if (!string.IsNullOrWhiteSpace(descripcion)) Descripcion = descripcion.Trim();
    }

    /// <summary>
    /// Sincroniza datos durante una importación masiva sin duplicar la referencia (Idempotencia).
    /// Evalúa si existen cambios reales antes de mutar la entidad.
    /// </summary>
    /// <param name="datos">Parámetros de sincronización provistos por el archivo.</param>
    /// <returns>True si al menos un atributo cambió; False si los datos eran idénticos.</returns>
    public bool SincronizarCargaMasiva(DatosSincronizacionProducto datos)
    {
        bool huboCambios = false;

        string nombreLimpio = datos.Nombre?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(nombreLimpio) && Nombre != nombreLimpio)
        {
            Nombre = nombreLimpio;
            huboCambios = true;
        }

        if (datos.PrecioVenta > 0 && PrecioVenta != datos.PrecioVenta)
        {
            PrecioVenta = datos.PrecioVenta;
            huboCambios = true;
        }

        if (datos.Costo > 0 && Costo != datos.Costo)
        {
            Costo = datos.Costo;
            huboCambios = true;
        }

        if (StockActual != datos.NuevoStock)
        {
            StockActual = datos.NuevoStock;
            huboCambios = true;
        }

        string ubicacionLimpia = datos.Ubicacion?.Trim().ToUpperInvariant() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(ubicacionLimpia) && Ubicacion != ubicacionLimpia)
        {
            Ubicacion = ubicacionLimpia;
            huboCambios = true;
        }

        string descripcionLimpia = datos.Descripcion?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(descripcionLimpia) && Descripcion != descripcionLimpia)
        {
            Descripcion = descripcionLimpia;
            huboCambios = true;
        }

        string marcaRepuestoLimpia = datos.MarcaRepuesto?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(marcaRepuestoLimpia) && MarcaRepuesto != marcaRepuestoLimpia)
        {
            MarcaRepuesto = marcaRepuestoLimpia;
            huboCambios = true;
        }

        string marcaVehiculoLimpia = datos.MarcaVehiculo?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(marcaVehiculoLimpia) && MarcaVehiculo != marcaVehiculoLimpia)
        {
            MarcaVehiculo = marcaVehiculoLimpia;
            huboCambios = true;
        }

        return huboCambios;
    }

    /// <summary>
    /// Actualiza la información básica del repuesto.
    /// </summary>
    /// <param name="nombre">Nuevo nombre comercial.</param>
    /// <param name="codigoParte">Nuevo código técnico.</param>
    /// <param name="categoriaId">Nuevo identificador de categoría.</param>
    public void ActualizarInformacion(string nombre, string codigoParte, Guid categoriaId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del repuesto no puede estar vacío.", nameof(nombre));
        }

        Nombre = nombre.Trim();
        CodigoParte = codigoParte?.Trim().ToUpperInvariant() ?? string.Empty;
        if (categoriaId != Guid.Empty)
        {
            CategoriaId = categoriaId;
        }
    }

    /// <summary>
    /// Actualiza el precio de venta del repuesto validando el rango permitido.
    /// </summary>
    /// <param name="nuevoPrecio">Nuevo precio de venta en COP.</param>
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
    /// <param name="cantidad">Cantidad a incorporar o restar.</param>
    public void AjustarStock(int cantidad)
    {
        int nuevoStock = StockActual + cantidad;
        if (nuevoStock < 0)
        {
            throw new InvalidOperationException($"No hay suficiente stock. Stock actual: {StockActual}, ajuste: {cantidad}.");
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