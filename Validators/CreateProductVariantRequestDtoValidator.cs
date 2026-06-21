using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class CreateProductVariantRequestDtoValidator : AbstractValidator<CreateProductVariantRequestDto>
    {
        public CreateProductVariantRequestDtoValidator()
        {
            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("SKU is required.")
                .MaximumLength(50)
                .WithMessage("SKU cannot exceed 50 characters.");

            RuleFor(x => x.Price)
                .InclusiveBetween(0, 1000000)
                .WithMessage("Price must be between 0 and 1000000.");

            RuleFor(x => x.QuantityInStock)
                .InclusiveBetween(0, 1000000)
                .WithMessage("Quantity in stock must be between 0 and 1000000.");

            RuleFor(x => x.Color)
                .MaximumLength(50)
                .WithMessage("Color cannot exceed 50 characters.");

            RuleFor(x => x.Size)
                .MaximumLength(50)
                .WithMessage("Size cannot exceed 50 characters.");
        }
    }
}