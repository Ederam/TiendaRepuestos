namespace TiendaRepuestos.Domain.Ports;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Puerto secundario que define las operaciones de persistencia y consulta para clientes.
/// </summary>
public interface IClienteRepository
{
    /// <summary>
    /// Obtiene un cliente por su identificador único.
    /// </summary>
    Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un cliente por su número exacto de documento (Cédula o NIT).
    /// </summary>
    Task<Cliente?> GetByDocumentoAsync(string documento, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca clientes que coincidan parcialmente por documento o nombre.
    /// </summary>
    Task<IReadOnlyCollection<Cliente>> SearchAsync(string termino, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra un nuevo cliente en el almacén de datos.
    /// </summary>
    Task AddAsync(Cliente cliente, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza la información de un cliente existente.
    /// </summary>
    Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default);
}