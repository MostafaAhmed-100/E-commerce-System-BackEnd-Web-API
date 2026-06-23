using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class RegisterAdminRequestDtoValidator : AbstractValidator<RegisterAdminRequestDto>
    {
        public RegisterAdminRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.AdminEmail)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredEmail])
                .EmailAddress().WithMessage(localizer[Constants.Resources.InvalidEmail]);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredPassword])
                .MinimumLength(7).WithMessage(localizer[Constants.Resources.MinLengthPassword]);

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredUsername])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxUsername]);

            RuleFor(x => x.AdminSecretCode)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredAdminSecretCode]);
        }
    }
}