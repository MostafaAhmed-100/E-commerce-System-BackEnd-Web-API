using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class AddSavedCardRequestValidator : AbstractValidator<AddSavedCardRequestDto>
    {
        public AddSavedCardRequestValidator()
        {
            RuleFor(x => x.CardBrand)
                .NotEmpty().WithMessage("The CardBrand cannot be empty")
                .MaximumLength(50).WithMessage("CardBrand cannot exceed 50 characters");

            RuleFor(x => x.MaskedNumber)
                .NotEmpty().WithMessage("The MaskedNumber cannot be empty")
                .MaximumLength(20).WithMessage("MaskedNumber cannot exceed 20 characters");

            RuleFor(x => x.CardToken)
                .NotEmpty().WithMessage("The CardToken cannot be empty")
                .MaximumLength(500).WithMessage("CardToken cannot exceed 500 characters");
        }
    }
}