using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class UpdateAddressRequestDtoValidator : AbstractValidator<UpdateAddressRequestDto>
    {
        public UpdateAddressRequestDtoValidator()
        {
            RuleFor(x => x.HomeAddress)
                .NotEmpty().WithMessage("Home address is required.")
                .MaximumLength(100).WithMessage("Home address cannot exceed 100 characters.");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100).WithMessage("State cannot exceed 100 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

            RuleFor(x => x.ZipCode)
               .NotEmpty().WithMessage("Zip Code is required.")
               .Matches("^[0-9]+$").WithMessage("Zip Code must contain only numbers.")
               .MaximumLength(10).WithMessage("Zip Code cannot exceed 10 characters.");
        }
    }
}