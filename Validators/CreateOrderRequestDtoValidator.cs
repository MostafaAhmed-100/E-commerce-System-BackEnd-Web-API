using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class CreateOrderRequestDtoValidator : AbstractValidator<CreateOrderRequestDto>
    {
        public CreateOrderRequestDtoValidator()
        {
            RuleFor(x => x.AddressId)
                .GreaterThan(0).WithMessage("Address ID must be greater than 0.");

            RuleFor(x => x.CouponCode)
                .MaximumLength(50).WithMessage("Coupon code cannot exceed 50 characters.");
        }
    }
}