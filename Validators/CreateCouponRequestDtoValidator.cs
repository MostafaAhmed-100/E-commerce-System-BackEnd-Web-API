using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.Entitys;

namespace WebApplication1.Validators
{
    public class CreateCouponRequestDtoValidator : AbstractValidator<CreateCouponRequestDto>
    {
        
        public CreateCouponRequestDtoValidator() 
        {
            RuleFor(C => C.CouponCode)
                .NotEmpty().WithMessage("Coupon code is required.")
                .MaximumLength(50).WithMessage("Coupon code cannot exceed 50 characters.");

            RuleFor(DT => DT.DiscountType)
                .IsInEnum().WithMessage("Invalid discount type.");

            RuleFor(DV => DV.DiscountValue)
                .GreaterThan(0).WithMessage("Discount value must be greater than 0.");

            RuleFor(SD => SD.StartDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past.");

            RuleFor(ED => ED.EndDate)
                .GreaterThan(SD => SD.StartDate).WithMessage("End date must be after the start date.");

            RuleFor(U => U.UsageLimit)
                .GreaterThan(0).WithMessage("Usage limit must be greater than 0.");
        }
    }
}
