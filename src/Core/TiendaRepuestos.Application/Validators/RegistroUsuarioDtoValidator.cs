namespace TiendaRepuestos.Application.Validators;

using FluentValidation;
using TiendaRepuestos.Application.DTOs.Auth;

/// <summary>
/// Validador de reglas para el registro de nuevos usuarios.
/// </summary>
public class RegistroUsuarioDtoValidator : AbstractValidator<RegistroUsuarioDto>
{
    /// <summary>
    /// Inicializa las reglas de validación para <see cref="RegistroUsuarioDto"/>.
    /// </summary>
    public RegistroUsuarioDtoValidator()
    {
        RuleFor(x => x.NombreCompleto)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El correo no tiene un formato válido.")
            .MaximumLength(100).WithMessage("El correo no puede superar los 100 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

        RuleFor(x => x.Rol)
            .IsInEnum().WithMessage("El rol especificado no es válido.");
    }
}