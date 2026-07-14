using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class AddWishlistItemDtoValidators : AbstractValidator<AddWishlistItemDto>
    {
        public AddWishlistItemDtoValidators(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product Id Is Required");
        }
    }
}
