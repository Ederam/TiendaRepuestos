namespace TiendaRepuestos.Domain.Enums;

/// <summary>
/// Define los niveles de acceso y roles permitidos en el sistema.
/// </summary>
public enum RolUsuario
{
    /// <summary>
    /// Rol operativo con permisos de facturación y consulta de stock.
    /// </summary>
    Vendedor = 1,

    /// <summary>
    /// Rol de gestión con permisos sobre catálogo, ajustes de inventario y reportes.
    /// </summary>
    Administrador = 2
}