using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class UpdateCartItemQuantityRequestDtoValidator : AbstractValidator<UpdateCartItemQuantityRequestDto>
    {
        public UpdateCartItemQuantityRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.Quantity)
                .InclusiveBetween(1, 100)
                .WithMessage(localizer[Constants.Resources.InvalidCartQuantity]);
        }
    }
}