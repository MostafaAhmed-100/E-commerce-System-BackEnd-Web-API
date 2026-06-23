using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class CreateProductRequestDtoValidator : AbstractValidator<CreateProductRequestDto>
    {
        public CreateProductRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredProductName])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxProductName]);

            RuleFor(x => x.ProductDescription)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredProductDesc])
                .MaximumLength(1000).WithMessage(localizer[Constants.Resources.MaxProductDesc]);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage(localizer[Constants.Resources.InvalidCategoryId]);

            RuleFor(x => x.Variants)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredVariants]);

            RuleForEach(x => x.Variants)
                .SetValidator(new CreateProductVariantRequestDtoValidator(localizer));
        }
    }
}