using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class UpdateCartItemQuantityRequestDtoValidator : AbstractValidator<UpdateCartItemQuantityRequestDto>
    {
        public UpdateCartItemQuantityRequestDtoValidator()
        {
            RuleFor(x => x.Quantity)
                .InclusiveBetween(1, 100)
                .WithMessage("Quantity must be between 1 and 100.");
        }
    }
}