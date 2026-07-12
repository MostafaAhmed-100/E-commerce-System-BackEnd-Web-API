using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredUsername])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxUsername]);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredEmail])
                .EmailAddress().WithMessage(localizer[Constants.Resources.InvalidEmail]);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredPassword])
                .MinimumLength(7).WithMessage(localizer[Constants.Resources.MinLengthPassword]); 

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredConfirmPassword])
                .Equal(x => x.Password).WithMessage(localizer[Constants.Resources.PasswordMismatch]);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredPhoneNumber])
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage(localizer[Constants.Resources.InvalidPhoneNumber]);
        }
    }
}