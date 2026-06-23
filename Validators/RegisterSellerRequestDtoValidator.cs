using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class RegisterSellerRequestDtoValidator : AbstractValidator<RegisterSellerRequestDto>
    {
        public RegisterSellerRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.SellerEmail)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredEmail])
                .EmailAddress().WithMessage(localizer[Constants.Resources.InvalidEmail]);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredPassword])
                .MinimumLength(7).WithMessage(localizer[Constants.Resources.MinLengthPassword]);

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredUsername])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxUsername]);

            RuleFor(x => x.BankName)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredBankName])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxBankName]);

            RuleFor(x => x.BankAccountNumber)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredBankAccount])
                .MaximumLength(90).WithMessage(localizer[Constants.Resources.MaxBankAccount]);

            RuleFor(x => x.StoreName)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredStoreName])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxStoreName]);

            RuleFor(x => x.TaxNumber)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredTaxNumber])
                .MaximumLength(50).WithMessage(localizer[Constants.Resources.MaxTaxNumber]);

            RuleFor(P => P.PhoneNumber)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredPhone])
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage(localizer[Constants.Resources.InvalidPhone]);
        }
    }
}