namespace TiendaRepuestos.Application.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.Common;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Enums;
using TiendaRepuestos.Domain.Ports;

/// <summary>
/// Servicio de aplicación que orquesta los casos de uso del catálogo de repuestos e inventario.
/// </summary>
public class ProductoService
{
    private readonly IProductoRepository _repository;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ProductoService"/> inyectando el repositorio de dominio.
    /// </summary>
    /// <param name="repository">Instancia del puerto de persistencia de productos.</param>
    /// <exception cref="ArgumentNullException">Se lanza si el repositorio es nulo.</exception>
    public ProductoService(IProductoRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Registra un nuevo repuesto en el inventario.
    /// </summary>
    /// <param name="dto">Datos para la creación del repuesto.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>El repuesto registrado proyectado en DTO.</returns>
    public async Task<ProductoResponseDto> CrearAsync(CrearProductoDto dto, CancellationToken cancellationToken = default)
    {
        Guid categoriaValidaId = dto.CategoriaId == Guid.Empty ? Guid.NewGuid() : dto.CategoriaId;

        Producto nuevoProducto = new Producto(
            dto.Nombre,
            dto.CodigoParte ?? string.Empty,
            dto.PrecioVenta,
            dto.StockInicial,
            dto.StockMinimo,
            dto.Estado,
            categoriaValidaId
        );

        await _repository.AddAsync(nuevoProducto, cancellationToken);
        return ProductoResponseDto.FromEntity(nuevoProducto);
    }

    /// <summary>
    /// Busca productos mediante coincidencia en nombre o código de parte.
    /// </summary>
    /// <param name="query">Criterio de búsqueda.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Colección de repuestos coincidentes.</returns>
    public async Task<IEnumerable<ProductoResponseDto>> BuscarAsync(string? query, CancellationToken cancellationToken = default)
    {
        string terminoBusqueda = query ?? string.Empty;
        IEnumerable<Producto> productosObtenidos = await _repository.SearchAsync(terminoBusqueda, cancellationToken);
        return productosObtenidos.Select(ProductoResponseDto.FromEntity);
    }

    /// <summary>
    /// Obtiene un producto por su identificador único.
    /// </summary>
    /// <param name="id">Identificador único del repuesto.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>El repuesto si existe; de lo contrario, null.</returns>
    public async Task<ProductoResponseDto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Producto? producto = await _repository.GetByIdAsync(id, cancellationToken);
        return producto is null ? null : ProductoResponseDto.FromEntity(producto);
    }

    /// <summary>
    /// Actualiza la información descriptiva y precio de un repuesto existente.
    /// </summary>
    /// <param name="id">Identificador único del producto.</param>
    /// <param name="dto">Datos actualizados.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>El repuesto con la información actualizada.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza si el repuesto no existe.</exception>
    public async Task<ProductoResponseDto> ActualizarAsync(Guid id, ActualizarProductoDto dto, CancellationToken cancellationToken = default)
    {
        Producto producto = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró el repuesto con ID: {id}");

        producto.ActualizarInformacion(dto.Nombre, dto.CodigoParte, dto.CategoriaId, dto.Costo);
        producto.ActualizarPrecio(dto.PrecioVenta);

        await _repository.UpdateAsync(producto, cancellationToken);
        return ProductoResponseDto.FromEntity(producto);
    }

    /// <summary>
    /// Ajusta las existencias físicas de un repuesto.
    /// </summary>
    /// <param name="id">Identificador único del repuesto.</param>
    /// <param name="cantidad">Cantidad a sumar o restar del stock.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>El repuesto con el saldo de inventario actualizado.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza si el repuesto no existe.</exception>
    public async Task<ProductoResponseDto> AjustarStockAsync(Guid id, int cantidad, CancellationToken cancellationToken = default)
    {
        Producto producto = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró el repuesto con ID: {id}");

        producto.AjustarStock(cantidad);

        await _repository.UpdateAsync(producto, cancellationToken);
        return ProductoResponseDto.FromEntity(producto);
    }

    /// <summary>
    /// Consulta el catálogo de repuestos aplicando paginación y criterios de filtrado.
    /// </summary>
    /// <param name="parameters">Parámetros de paginación y filtros.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Resultado paginado con la colección de repuestos y metadatos de navegación.</returns>
    public async Task<PagedResult<ProductoResponseDto>> ObtenerPaginadoAsync(
        ConsultaProductosParameters parameters,
        CancellationToken cancellationToken = default)
    {
        (IReadOnlyCollection<Producto> items, int totalCount) = await _repository.ObtenerPaginadoAsync(
            parameters.Busqueda,
            parameters.CategoriaId,
            parameters.SoloActivos,
            parameters.PageNumber,
            parameters.PageSize,
            cancellationToken);

        List<ProductoResponseDto> dtos = items.Select(ProductoResponseDto.FromEntity).ToList();

        return new PagedResult<ProductoResponseDto>(dtos, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    /// <summary>
    /// Procesa una carga masiva de repuestos realizando un Upsert idempotente mediante la clave compuesta Origen + Código de Parte.
    /// </summary>
    /// <param name="items">Colección de registros entrantes desde archivo.</param>
    /// <param name="cancellationToken">Token de cancelación cooperativa.</param>
    /// <returns>Resumen del procesamiento con cantidades de creados, actualizados y sin cambios.</returns>
    public async Task<CargaMasivaResultadoDto> CargaMasivaAsync(
        IReadOnlyCollection<CargaMasivaItemDto> items,
        CancellationToken cancellationToken = default)
    {
        if (items == null || items.Count == 0)
        {
            return new CargaMasivaResultadoDto(0, 0, 0, 0, 0, new List<string> { "No se proporcionaron registros para procesar." });
        }

        List<string> codigos = items.Select(i => i.CodigoParte).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();
        List<string> origenes = items.Select(i => i.Origen ?? "PROPIO_MACROLATAS").Distinct().ToList();

        IReadOnlyCollection<Producto> existentes = await _repository.ObtenerPorCodigosYOrigenesAsync(codigos, origenes, cancellationToken);
        Dictionary<string, Producto> mapaExistentes = existentes.ToDictionary(p => GenerarClaveCompuesta(p.Origen, p.CodigoParte));

        int insertados = 0;
        int actualizados = 0;
        int sinCambios = 0;
        int omitidos = 0;
        List<Producto> nuevosParaInsertar = new List<Producto>();

        foreach (CargaMasivaItemDto item in items)
        {
            if (string.IsNullOrWhiteSpace(item.CodigoParte) || string.IsNullOrWhiteSpace(item.Nombre))
            {
                omitidos++;
                continue;
            }

            string clave = GenerarClaveCompuesta(item.Origen, item.CodigoParte);

            if (mapaExistentes.TryGetValue(clave, out Producto? productoExistente))
            {
                bool fueModificado = ActualizarProductoExistente(productoExistente, item);
                if (fueModificado)
                {
                    actualizados++;
                }
                else
                {
                    sinCambios++;
                }
            }
            else
            {
                Producto nuevoProducto = CrearNuevoProducto(item);
                nuevosParaInsertar.Add(nuevoProducto);
                mapaExistentes.Add(clave, nuevoProducto);
                insertados++;
            }
        }

        await _repository.GuardarCambiosLoteAsync(nuevosParaInsertar, cancellationToken);

        List<string> mensajes = new List<string>
        {
            $"Proceso completado exitosamente: {insertados} creados, {actualizados} actualizados, {sinCambios} sin cambios, {omitidos} omitidos."
        };

        return new CargaMasivaResultadoDto(
            TotalProcesados: items.Count,
            Insertados: insertados,
            Actualizados: actualizados,
            SinCambios: sinCambios,
            Omitidos: omitidos,
            Mensajes: mensajes
        );
    }

    /// <summary>
    /// Genera la clave compuesta para el índice en memoria en tiempo O(1).
    /// </summary>
    private static string GenerarClaveCompuesta(string? origen, string codigoParte)
    {
        string origenNormalizado = (origen ?? "PROPIO_MACROLATAS").Trim().ToUpperInvariant();
        string codigoNormalizado = codigoParte.Trim().ToUpperInvariant();
        return $"{origenNormalizado}::{codigoNormalizado}";
    }

    /// <summary>
    /// Aplica la actualización de valores sobre un repuesto ya existente validando si hubo cambios reales.
    /// </summary>
    private static bool ActualizarProductoExistente(Producto producto, CargaMasivaItemDto dto)
    {
        DatosSincronizacionProducto datosSincronizacion = new DatosSincronizacionProducto(
            Nombre: dto.Nombre,
            PrecioVenta: dto.PrecioVenta,
            Costo: dto.Costo,
            NuevoStock: dto.StockActual,
            Ubicacion: dto.Ubicacion,
            Descripcion: dto.Descripcion,
            MarcaRepuesto: dto.MarcaRepuesto,
            MarcaVehiculo: dto.MarcaVehiculo
        );

        return producto.SincronizarCargaMasiva(datosSincronizacion);
    }

    /// <summary>
    /// Construye una nueva instancia de la entidad Producto aplicando metadatos complementarios sin violar reglas de arquitectura limpia.
    /// </summary>
    private static Producto CrearNuevoProducto(CargaMasivaItemDto dto)
    {
        Producto nuevo = new Producto(
            dto.Nombre,
            dto.CodigoParte,
            dto.PrecioVenta,
            dto.StockActual,
            2,
            EstadoProducto.Nuevo,
            dto.CategoriaId ?? Guid.Empty
        );

        nuevo.AsignarMetadatosCatalogo(
            dto.Origen,
            dto.Ubicacion,
            dto.MarcaRepuesto,
            dto.MarcaVehiculo,
            dto.Condicion,
            dto.Costo,
            dto.Descripcion
        );

        return nuevo;
    }
}