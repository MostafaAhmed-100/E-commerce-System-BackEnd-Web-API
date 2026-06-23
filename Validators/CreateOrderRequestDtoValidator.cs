using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class CreateOrderRequestDtoValidator : AbstractValidator<CreateOrderRequestDto>
    {
        public CreateOrderRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.AddressId)
                .GreaterThan(0).WithMessage(localizer[Constants.Resources.InvalidAddressId]);

            RuleFor(x => x.CouponCode)
                .MaximumLength(50).WithMessage(localizer[Constants.Resources.MaxCouponCode]);
        }
    }
}