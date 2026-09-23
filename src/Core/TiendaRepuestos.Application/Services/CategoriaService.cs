namespace TiendaRepuestos.Application.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Application.DTOs;
using TiendaRepuestos.Domain.Entities;
using TiendaRepuestos.Domain.Ports;

public class CategoriaService
{
    private readonly ICategoriaRepository _repository;

    public CategoriaService(ICategoriaRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<CategoriaResponseDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Categoria> categorias = await _repository.GetAllAsync(cancellationToken);
        return categorias.Select(CategoriaResponseDto.FromEntity);
    }

    public async Task<CategoriaResponseDto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Categoria? categoria = await _repository.GetByIdAsync(id, cancellationToken);
        return categoria is null ? null : CategoriaResponseDto.FromEntity(categoria);
    }

    public async Task<CategoriaResponseDto> CrearAsync(CrearCategoriaDto dto, CancellationToken cancellationToken = default)
    {
        Categoria nuevaCategoria = new Categoria(dto.Nombre, dto.Descripcion);
        await _repository.AddAsync(nuevaCategoria, cancellationToken);
        return CategoriaResponseDto.FromEntity(nuevaCategoria);
    }
}