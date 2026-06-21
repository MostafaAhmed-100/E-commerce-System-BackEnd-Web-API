using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class UpdateCouponRequestDtoValidator : AbstractValidator<UpdateCouponRequestDto>
    {
        public UpdateCouponRequestDtoValidator()
        {
            RuleFor(x => x.DiscountType)
                .IsInEnum().WithMessage("Invalid discount type.");

            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage("Discount value must be greater than 0.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after the start date.");

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0).WithMessage("Usage limit must be greater than 0.");
        }
    }
}