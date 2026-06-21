using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class RegisterAdminRequestDtoValidator : AbstractValidator<RegisterAdminRequestDto>
    {
        public RegisterAdminRequestDtoValidator()
        {
            RuleFor(x => x.AdminEmail)
                .NotEmpty().WithMessage("Admin email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(7).WithMessage("Password must be at least 7 characters long.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(100).WithMessage("Username cannot exceed 100 characters.");

            RuleFor(x => x.AdminSecretCode)
                .NotEmpty().WithMessage("Admin secret code is required.");
        }
    }
}