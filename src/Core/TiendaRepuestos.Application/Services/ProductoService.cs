namespace TiendaRepuestos.Application.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Ports;

/// <summary>
/// Servicio de aplicación que coordina los casos de uso relativos a la gestión de repuestos.
/// </summary>
public class ProductoService
{
    private readonly IProductoRepository _repository;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ProductoService"/>.
    /// </summary>
    /// <param name="repository">Instancia del repositorio del puerto de productos.</param>
    public ProductoService(IProductoRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Registra un nuevo repuesto en el inventario garantizando la validación de dominio.
    /// </summary>
    /// <param name="dto">Objeto con los datos de creación del repuesto.</param>
    /// <param name="cancellationToken">Token para la cancelación asíncrona de la operación.</param>
    /// <returns>DTO con la información del repuesto creado y su ID asignado.</returns>
    public async Task<ProductoResponseDto> CrearAsync(CrearProductoDto dto, CancellationToken cancellationToken = default)
    {
        Guid categoriaValidaId = dto.CategoriaId == Guid.Empty ? Guid.NewGuid() : dto.CategoriaId;

        Producto nuevoProducto = new Producto(
            dto.Nombre,
            dto.CodigoParte ?? string.Empty,
            dto.PrecioVenta,
            dto.StockInicial,
            dto.Estado,
            categoriaValidaId
        );

        await _repository.AddAsync(nuevoProducto, cancellationToken);

        ProductoResponseDto respuesta = ProductoResponseDto.FromEntity(nuevoProducto);
        return respuesta;
    }

    /// <summary>
    /// Busca repuestos por coincidencia en nombre o código de parte/referencia.
    /// </summary>
    /// <param name="query">Término de búsqueda ingresado por el usuario.</param>
    /// <param name="cancellationToken">Token para la cancelación asíncrona de la operación.</param>
    /// <returns>Colección de DTOs con los repuestos encontrados.</returns>
    public async Task<IEnumerable<ProductoResponseDto>> BuscarAsync(string? query, CancellationToken cancellationToken = default)
    {
        string terminoBusqueda = query ?? string.Empty;

        IEnumerable<Producto> productosObtenidos = await _repository.SearchAsync(terminoBusqueda, cancellationToken);

        IEnumerable<ProductoResponseDto> listaRespuesta = productosObtenidos.Select(ProductoResponseDto.FromEntity);
        return listaRespuesta;
    }
}