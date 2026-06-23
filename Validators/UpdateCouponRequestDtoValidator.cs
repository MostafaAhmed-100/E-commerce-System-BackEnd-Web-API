using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class UpdateCouponRequestDtoValidator : AbstractValidator<UpdateCouponRequestDto>
    {
        public UpdateCouponRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.DiscountType)
                .IsInEnum().WithMessage(localizer[Constants.Resources.InvalidDiscountType]);

            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage(localizer[Constants.Resources.InvalidDiscountValue]);

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredStartDate]);

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredEndDate])
                .GreaterThan(x => x.StartDate).WithMessage(localizer[Constants.Resources.InvalidEndDate]);

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0).WithMessage(localizer[Constants.Resources.InvalidUsageLimit]);
        }
    }
}