namespace TiendaRepuestos.Application.Validators;

using FluentValidation;
using TiendaRepuestos.Application.DTOs;

/// <summary>
/// Validador de reglas de negocio de entrada para la creación de comprobantes de venta.
/// </summary>
public class CrearVentaDtoValidator : AbstractValidator<CrearVentaDto>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="CrearVentaDtoValidator"/> y define sus reglas.
    /// </summary>
    public CrearVentaDtoValidator()
    {
        RuleFor(x => x.NumeroComprobante)
            .NotEmpty().WithMessage("El número de comprobante es obligatorio.")
            .MaximumLength(20).WithMessage("El número de comprobante no puede superar los 20 caracteres.")
            .Matches(@"^VTA-\d{4,}$").WithMessage("El número de comprobante debe tener el formato 'VTA-XXXX' (ejemplo: VTA-0001).");

        RuleFor(x => x.MetodoPago)
            .IsInEnum().WithMessage("El método de pago especificado no es válido.");

        RuleFor(x => x.Detalles)
            .NotEmpty().WithMessage("La venta debe contener al menos un producto.")
            .Must(detalles => detalles != null && detalles.Count > 0).WithMessage("Debe incluir al menos un ítem en el detalle de la venta.");

        RuleForEach(x => x.Detalles)
            .SetValidator(new CrearDetalleVentaDtoValidator());
    }
}

/// <summary>
/// Validador para cada ítem individual dentro del detalle de la venta.
/// </summary>
public class CrearDetalleVentaDtoValidator : AbstractValidator<CrearDetalleVentaDto>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="CrearDetalleVentaDtoValidator"/>.
    /// </summary>
    public CrearDetalleVentaDtoValidator()
    {
        RuleFor(x => x.ProductoId)
            .NotEmpty().WithMessage("El ID del producto es obligatorio.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad a vender debe ser mayor a cero.");
    }
}