using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
    {
        public ChangePasswordRequestValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredPassword])
                .MinimumLength(7).WithMessage(localizer[Constants.Resources.MinLengthPassword]);

            RuleFor(x => x.NewPassword)
                 .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredPassword])
                 .MinimumLength(7).WithMessage(localizer[Constants.Resources.MinLengthPassword]);

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredConfirmPassword])
                .Equal(x => x.NewPassword).WithMessage(localizer[Constants.Resources.PasswordMismatch]);
        }
    }
}
