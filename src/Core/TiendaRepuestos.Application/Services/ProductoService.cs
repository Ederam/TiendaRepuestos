namespace TiendaRepuestos.Application.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Ports;

public class ProductoService
{
    private readonly IProductoRepository _repository;

    public ProductoService(IProductoRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

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

    public async Task<IEnumerable<ProductoResponseDto>> BuscarAsync(string? query, CancellationToken cancellationToken = default)
    {
        string terminoBusqueda = query ?? string.Empty;
        IEnumerable<Producto> productosObtenidos = await _repository.SearchAsync(terminoBusqueda, cancellationToken);
        return productosObtenidos.Select(ProductoResponseDto.FromEntity);
    }

    public async Task<ProductoResponseDto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Producto? producto = await _repository.GetByIdAsync(id, cancellationToken);
        return producto is null ? null : ProductoResponseDto.FromEntity(producto);
    }

    public async Task<ProductoResponseDto> ActualizarAsync(Guid id, ActualizarProductoDto dto, CancellationToken cancellationToken = default)
    {
        Producto producto = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró el repuesto con ID: {id}");

        producto.ActualizarInformacion(dto.Nombre, dto.CodigoParte, dto.CategoriaId);
        producto.ActualizarPrecio(dto.PrecioVenta);

        await _repository.UpdateAsync(producto, cancellationToken);
        return ProductoResponseDto.FromEntity(producto);
    }

    public async Task<ProductoResponseDto> AjustarStockAsync(Guid id, int cantidad, CancellationToken cancellationToken = default)
    {
        Producto producto = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró el repuesto con ID: {id}");

        producto.AjustarStock(cantidad);

        await _repository.UpdateAsync(producto, cancellationToken);
        return ProductoResponseDto.FromEntity(producto);
    }
}