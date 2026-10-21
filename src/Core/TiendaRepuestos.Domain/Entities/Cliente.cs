namespace TiendaRepuestos.Domain.Entities;

using System;

/// <summary>
/// Entidad de dominio que representa un cliente comercial o adquirente de factura electrónica.
/// </summary>
public class Cliente
{
    /// <summary>
    /// Identificador único global del cliente.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Número de Cédula de Ciudadanía, Extranjería o NIT con dígito de verificación.
    /// </summary>
    public string Documento { get; private set; } = string.Empty;

    /// <summary>
    /// Nombres y apellidos o razón social registrada ante la DIAN.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Teléfono o celular de contacto comercial.
    /// </summary>
    public string Telefono { get; private set; } = string.Empty;

    /// <summary>
    /// Correo electrónico para entrega del XML y representación gráfica de la Factura Electrónica.
    /// </summary>
    public string Correo { get; private set; } = string.Empty;

    /// <summary>
    /// Dirección fiscal física o de taller/oficina.
    /// </summary>
    public string Direccion { get; private set; } = string.Empty;

    /// <summary>
    /// Indica si el cliente tiene habilitación para emisión de Facturación Electrónica obligatoria.
    /// </summary>
    public bool EsFacturaElectronica { get; private set; }

    /// <summary>
    /// Estado de actividad lógica del cliente en el catálogo.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Constructor protegido para el mapeo relacional de Entity Framework Core.
    /// </summary>
    private Cliente() { }

    /// <summary>
    /// Inicializa una nueva instancia de cliente con validaciones de invariantes de negocio.
    /// </summary>
    /// <param name="documento">Número de identificación o NIT.</param>
    /// <param name="nombre">Nombre comercial o razón social.</param>
    /// <param name="telefono">Teléfono de contacto.</param>
    /// <param name="correo">Correo electrónico para notificación fiscal.</param>
    /// <param name="direccion">Ubicación o dirección física.</param>
    /// <param name="esFacturaElectronica">Bandera de requerimiento fiscal DIAN.</param>
    public Cliente(
        string documento,
        string nombre,
        string? telefono,
        string? correo,
        string? direccion,
        bool esFacturaElectronica)
    {
        if (string.IsNullOrWhiteSpace(documento))
        {
            throw new ArgumentException("El documento de identificación del cliente no puede estar vacío.", nameof(documento));
        }

        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre o razón social del cliente no puede estar vacío.", nameof(nombre));
        }

        Id = Guid.NewGuid();
        Documento = documento.Trim().ToUpperInvariant();
        Nombre = nombre.Trim();
        Telefono = telefono?.Trim() ?? string.Empty;
        Correo = correo?.Trim().ToLowerInvariant() ?? string.Empty;
        Direccion = direccion?.Trim() ?? string.Empty;
        EsFacturaElectronica = esFacturaElectronica;
        Activo = true;
    }

    /// <summary>
    /// Actualiza los datos de contacto y régimen fiscal del cliente.
    /// </summary>
    public void ActualizarDatos(
        string nombre,
        string? telefono,
        string? correo,
        string? direccion,
        bool esFacturaElectronica)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del cliente no puede estar vacío.", nameof(nombre));
        }

        Nombre = nombre.Trim();
        Telefono = telefono?.Trim() ?? string.Empty;
        Correo = correo?.Trim().ToLowerInvariant() ?? string.Empty;
        Direccion = direccion?.Trim() ?? string.Empty;
        EsFacturaElectronica = esFacturaElectronica;
    }

    /// <summary>
    /// Desactiva lógicamente al cliente del directorio operativo.
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
    }
}