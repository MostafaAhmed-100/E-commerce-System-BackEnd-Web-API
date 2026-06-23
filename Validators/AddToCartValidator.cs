using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;
namespace WebApplication1.Validators
{
    public class AddToCartRequestDtoValidator : AbstractValidator<AddToCartRequestDto>
    {
        public AddToCartRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.ProductVariantId)
                .GreaterThan(0).WithMessage(localizer[Constants.Resources.InvalidVariantId]);

            RuleFor(x => x.Quantity)
                .InclusiveBetween(1, 100).WithMessage(localizer[Constants.Resources.InvalidCartQuantity]);
        }
    }
}