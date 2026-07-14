using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class CreateWishlistDtoValidators : AbstractValidator<CreateWishlistRequestDto>
    {
        public CreateWishlistDtoValidators(IStringLocalizer<SharedResource> localizer) 
        {
            RuleFor(x => x.WishlistName)
                .NotEmpty().WithMessage("Wish List Name is needed")
                .MaximumLength(100).WithMessage("max length is 100");
        }
    }
}
