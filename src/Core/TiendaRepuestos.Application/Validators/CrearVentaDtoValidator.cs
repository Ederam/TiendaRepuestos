namespace TiendaRepuestos.Application.Validators;

using FluentValidation;
using TiendaRepuestos.Application.DTOs;

/// <summary>
/// Validador de reglas de negocio de entrada para procesar ventas de mostrador y facturación.
/// </summary>
public class CrearVentaDtoValidator : AbstractValidator<CrearVentaDto>
{
    /// <summary>
    /// Define las restricciones de integridad sobre el método de pago, ítems y cliente.
    /// </summary>
    public CrearVentaDtoValidator()
    {
        RuleFor(x => x.MetodoPago)
            .IsInEnum().WithMessage("El método de pago especificado no es válido (1: Efectivo, 2: Transferencia, 3: Tarjeta).");

        RuleFor(x => x.Detalles)
            .NotNull().WithMessage("La lista de detalles no puede ser nula.")
            .NotEmpty().WithMessage("La venta debe contener al menos un producto.");

        RuleForEach(x => x.Detalles)
            .SetValidator(new CrearDetalleVentaDtoValidator());

        When(x => !string.IsNullOrWhiteSpace(x.ClienteDocumento), () =>
        {
            RuleFor(x => x.ClienteDocumento)
                .MaximumLength(20).WithMessage("El documento del cliente no puede exceder los 20 caracteres.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.ClienteNombre), () =>
        {
            RuleFor(x => x.ClienteNombre)
                .MaximumLength(150).WithMessage("El nombre del cliente no puede exceder los 150 caracteres.");
        });
    }
}

/// <summary>
/// Validador individual para cada línea de detalle de la venta.
/// </summary>
public class CrearDetalleVentaDtoValidator : AbstractValidator<CrearDetalleVentaDto>
{
    /// <summary>
    /// Configura las validaciones de producto y volumen de venta.
    /// </summary>
    public CrearDetalleVentaDtoValidator()
    {
        RuleFor(x => x.ProductoId)
            .NotEmpty().WithMessage("El identificador único (GUID) del producto es obligatorio.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad a facturar debe ser superior a cero.");
    }
}