namespace TiendaRepuestos.Application.Validators;

using FluentValidation;
using TiendaRepuestos.Application.DTOs.Auth;

/// <summary>
/// Validador de entrada para solicitudes de inicio de sesión.
/// </summary>
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    /// <summary>
    /// Inicializa las reglas de validación para <see cref="LoginDto"/>.
    /// </summary>
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El formato del correo electrónico es inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}