using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class UpdateSellerProfileRequestDtoValidator : AbstractValidator<UpdateSellerProfileRequestDto>
    {
        public UpdateSellerProfileRequestDtoValidator(IStringLocalizer<SharedResource> localizer) 
        {
            RuleFor(x => x.SellerBankName)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredBankName])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxBankName]);

            RuleFor(x => x.SellerBankAccountNumber)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredBankAccount])
                .MaximumLength(90).WithMessage(localizer[Constants.Resources.MaxBankAccount]);

            RuleFor(x => x.SellerStoreName)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredStoreName])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxStoreName]);

            RuleFor(P => P.SellerPhoneNumber)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredPhoneNumber])
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage(localizer[Constants.Resources.InvalidPhoneNumber]);
        }
    }
}
