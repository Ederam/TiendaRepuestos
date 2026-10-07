namespace TiendaRepuestos.Domain.Ports;

using System;
using System.Threading;
using System.Threading.Tasks;
using TiendaRepuestos.Domain.Entities;

/// <summary>
/// Puerto para la persistencia y consulta de cuentas de usuario.
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>
    /// Busca un usuario por su dirección de correo electrónico.
    /// </summary>
    Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra un nuevo usuario en la base de datos.
    /// </summary>
    Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default);
}