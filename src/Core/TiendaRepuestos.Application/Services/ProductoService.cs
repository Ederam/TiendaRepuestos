namespace TiendaRepuestos.Application.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.Common;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Domain.Entities;
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

        producto.ActualizarInformacion(dto.Nombre, dto.CodigoParte, dto.CategoriaId);
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
}