using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class UpdateBuyerProfileRequestDtoValidator : AbstractValidator<UpdateBuyerProfileRequestDto>
    {
        public UpdateBuyerProfileRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.BuyerName)
                 .NotEmpty()
                .WithMessage(localizer[Constants.Resources.RequiredUsername])
                .MaximumLength(100)
                .WithMessage(localizer[Constants.Resources.MaxUsername]);


            RuleFor(x => x.BuyerPhoneNumber)
               .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredPhoneNumber])
               .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage(localizer[Constants.Resources.InvalidPhoneNumber]);
        }
    }
}
