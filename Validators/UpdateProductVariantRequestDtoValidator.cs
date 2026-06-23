using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class UpdateProductVariantRequestDtoValidator : AbstractValidator<UpdateProductVariantRequestDto>
    {
        public UpdateProductVariantRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.VariantId)
                .GreaterThan(0).WithMessage(localizer[Constants.Resources.InvalidVariantId]);

            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredSKU])
                .MaximumLength(50).WithMessage(localizer[Constants.Resources.MaxSKU]);

            RuleFor(x => x.Price)
                .InclusiveBetween(0, 1000000).WithMessage(localizer[Constants.Resources.InvalidPrice]);

            RuleFor(x => x.QuantityInStock)
                .InclusiveBetween(0, 1000000).WithMessage(localizer[Constants.Resources.InvalidStock]);

            RuleFor(x => x.Color)
                .MaximumLength(50).WithMessage(localizer[Constants.Resources.MaxColor]);

            RuleFor(x => x.Size)
                .MaximumLength(50).WithMessage(localizer[Constants.Resources.MaxSize]);
        }
    }
}