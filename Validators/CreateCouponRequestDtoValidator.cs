using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class CreateCouponRequestDtoValidator : AbstractValidator<CreateCouponRequestDto>
    {
        public CreateCouponRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(C => C.CouponCode)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredCouponCode])
                .MaximumLength(50).WithMessage(localizer[Constants.Resources.MaxCouponCode]);

            RuleFor(DT => DT.DiscountType)
                .IsInEnum().WithMessage(localizer[Constants.Resources.InvalidDiscountType]);

            RuleFor(DV => DV.DiscountValue)
                .GreaterThan(0).WithMessage(localizer[Constants.Resources.InvalidDiscountValue]);

            RuleFor(SD => SD.StartDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage(localizer[Constants.Resources.InvalidStartDate]);

            RuleFor(ED => ED.EndDate)
                .GreaterThan(SD => SD.StartDate).WithMessage(localizer[Constants.Resources.InvalidEndDate]);

            RuleFor(U => U.UsageLimit)
                .GreaterThan(0).WithMessage(localizer[Constants.Resources.InvalidUsageLimit]);
        }
    }
}