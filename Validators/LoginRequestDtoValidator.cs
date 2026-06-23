using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredEmail])
                .EmailAddress().WithMessage(localizer[Constants.Resources.InvalidEmail]);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredPassword])
                .MinimumLength(7).WithMessage(localizer[Constants.Resources.MinLengthPassword]);
        }
    }
}