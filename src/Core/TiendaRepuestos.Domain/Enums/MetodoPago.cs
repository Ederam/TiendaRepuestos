namespace TiendaRepuestos.Domain.Enums;

/// <summary>
/// Define las formas de pago aceptadas en la caja registradora.
/// </summary>
public enum MetodoPago
{
    Efectivo = 1,
    TarjetaDebito = 2,
    TarjetaCredito = 3,
    Transferencia = 4
}