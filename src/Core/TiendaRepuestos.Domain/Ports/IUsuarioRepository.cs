namespace TiendaRepuestos.Domain.Ports;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Puerto para la persistencia y consulta de cuentas de usuario.
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>
    /// Busca un usuario por su identificador único (GUID).
    /// </summary>
    Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca un usuario por su dirección de correo electrónico.
    /// </summary>
    Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el listado de usuarios activos del sistema (vendedores y administradores) para el filtro de auditoría.
    /// </summary>
    Task<IReadOnlyList<Usuario>> ObtenerVendedoresActivosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra un nuevo usuario en la base de datos.
    /// </summary>
    Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default);
}