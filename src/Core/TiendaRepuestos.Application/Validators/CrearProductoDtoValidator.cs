

using FluentValidation;
using TiendaRepuestos.Application.DTOs;

namespace TiendaRepuestos.Application.Validators
{
    /// <summary>
    /// validador de reglas de negocio de entrada para la creación de un nuevo repuesto en el inventario.
    /// </summary>
    public class CrearProductoDtoValidator : AbstractValidator<CrearProductoDto>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// </summary>
        public CrearProductoDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre del producto no puede superar los 100 caracteres.");
            RuleFor(x => x.CodigoParte)
                .MaximumLength(50).WithMessage("El código de parte no puede superar los 50 caracteres.");
            RuleFor(x => x.PrecioVenta)
                .GreaterThan(0).WithMessage("El precio de venta debe ser mayor a cero.");
            RuleFor(x => x.StockInicial)
                .GreaterThanOrEqualTo(0).WithMessage("El stock inicial no puede ser negativo.");
            RuleFor(x => x.StockMinimo)
                .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");
            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("El estado del producto especificado no es válido.");
            RuleFor(x => x.CategoriaId)
                .NotEmpty().WithMessage("La categoría del producto es obligatoria.");
        }
    }
}
