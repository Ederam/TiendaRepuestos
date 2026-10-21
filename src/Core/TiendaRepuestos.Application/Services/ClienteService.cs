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
/// Servicio de aplicación que orquesta los casos de uso de gestión y consulta de clientes.
/// </summary>
public class ClienteService
{
    private readonly IClienteRepository _repository;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ClienteService"/>.
    /// </summary>
    /// <param name="repository">Instancia del repositorio de clientes.</param>
    public ClienteService(IClienteRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Registra un nuevo cliente validando que el documento no esté duplicado.
    /// </summary>
    public async Task<ClienteResponseDto> CrearAsync(CrearClienteDto dto, CancellationToken cancellationToken = default)
    {
        Cliente? existente = await _repository.GetByDocumentoAsync(dto.Documento, cancellationToken);
        if (existente is not null)
        {
            // Si ya existe, actualiza sus datos y lo retorna sin fallar (Idempotencia en mostrador)
            existente.ActualizarDatos(
                dto.Nombre,
                dto.Telefono,
                dto.Correo,
                dto.Direccion,
                dto.EsFacturaElectronica ?? false
            );
            await _repository.UpdateAsync(existente, cancellationToken);
            return ClienteResponseDto.FromEntity(existente);
        }

        Cliente nuevo = new Cliente(
            dto.Documento,
            dto.Nombre,
            dto.Telefono,
            dto.Correo,
            dto.Direccion,
            dto.EsFacturaElectronica ?? false
        );

        await _repository.AddAsync(nuevo, cancellationToken);
        return ClienteResponseDto.FromEntity(nuevo);
    }

    /// <summary>
    /// Busca clientes por coincidencia parcial en documento o nombre.
    /// </summary>
    public async Task<IEnumerable<ClienteResponseDto>> BuscarAsync(string? termino, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termino))
        {
            return Enumerable.Empty<ClienteResponseDto>();
        }

        IReadOnlyCollection<Cliente> resultados = await _repository.SearchAsync(termino.Trim(), cancellationToken);
        return resultados.Select(ClienteResponseDto.FromEntity);
    }

    /// <summary>
    /// Consulta un cliente puntual por su identificador único.
    /// </summary>
    public async Task<ClienteResponseDto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Cliente? cliente = await _repository.GetByIdAsync(id, cancellationToken);
        return cliente is null ? null : ClienteResponseDto.FromEntity(cliente);
    }

    /// <summary>
    /// Consulta un cliente puntual por su número de Cédula o NIT.
    /// </summary>
    public async Task<ClienteResponseDto?> ObtenerPorDocumentoAsync(string documento, CancellationToken cancellationToken = default)
    {
        Cliente? cliente = await _repository.GetByDocumentoAsync(documento, cancellationToken);
        return cliente is null ? null : ClienteResponseDto.FromEntity(cliente);
    }
}