using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class CreateProductRequestDtoValidator : AbstractValidator<CreateProductRequestDto>
    {
        public CreateProductRequestDtoValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

            RuleFor(x => x.ProductDescription)
                .NotEmpty().WithMessage("Product description is required.")
                .MaximumLength(1000).WithMessage("Product description cannot exceed 1000 characters.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0.");

            RuleFor(x => x.Variants)
                .NotEmpty()
                .WithMessage("Product Variants is required.");
            RuleForEach(x => x.Variants)
                .SetValidator(new CreateProductVariantRequestDtoValidator());
        }
    }
}