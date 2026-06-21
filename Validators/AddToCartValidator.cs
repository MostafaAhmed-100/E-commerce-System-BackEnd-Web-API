using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class AddToCartRequestDtoValidator : AbstractValidator<AddToCartRequestDto>
    {
        public AddToCartRequestDtoValidator()
        {
            RuleFor(x => x.ProductVariantId)
                .GreaterThan(0).WithMessage("Product Variant ID must be greater than 0.");

            RuleFor(x => x.Quantity)
                .InclusiveBetween(1, 100).WithMessage("Quantity must be between 1 and 100.");
        }
    }
}