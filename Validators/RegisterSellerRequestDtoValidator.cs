using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class RegisterSellerRequestDtoValidator : AbstractValidator<RegisterSellerRequestDto>
    {
        public RegisterSellerRequestDtoValidator()
        {
            RuleFor(x => x.SellerEmail)
                .NotEmpty().WithMessage("Seller email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(7).WithMessage("Password must be at least 7 characters long.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(100).WithMessage("Username cannot exceed 100 characters.");

            RuleFor(x => x.BankName)
                .NotEmpty().WithMessage("Bank name is required.")
                .MaximumLength(100).WithMessage("Bank name cannot exceed 100 characters.");

            RuleFor(x => x.BankAccountNumber)
                .NotEmpty().WithMessage("Bank account number is required.")
                .MaximumLength(90).WithMessage("Bank account number cannot exceed 90 characters.");

            RuleFor(x => x.StoreName)
                .NotEmpty().WithMessage("Store name is required.")
                .MaximumLength(100).WithMessage("Store name cannot exceed 100 characters.");

            RuleFor(x => x.TaxNumber)
                .NotEmpty().WithMessage("Tax number is required.")
                .MaximumLength(50).WithMessage("Tax number cannot exceed 50 characters.");

            RuleFor(P => P.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Invalid phone number format.");


        }
    }
}