namespace TiendaRepuestos.Domain.Constants;

/// <summary>
/// Define las constantes estáticas de roles para uso en decoradores de autorización y políticas de seguridad.
/// </summary>
public static class Roles
{
    /// <summary>
    /// Rol de gestión con permisos totales sobre el catálogo, inventario y configuración.
    /// </summary>
    public const string Administrador = "Administrador";

    /// <summary>
    /// Rol operativo con permisos sobre facturación y consulta de existencias en el punto de venta.
    /// </summary>
    public const string Vendedor = "Vendedor";

    /// <summary>
    /// Agrupación combinada para endpoints accesibles por ambos roles.
    /// </summary>
    public const string Todos = Administrador + "," + Vendedor;
}