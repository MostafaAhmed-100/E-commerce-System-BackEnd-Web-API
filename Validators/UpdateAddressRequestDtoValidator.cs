using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class UpdateAddressRequestDtoValidator : AbstractValidator<UpdateAddressRequestDto>
    {
        public UpdateAddressRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.HomeAddress)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredHomeAddress])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxHomeAddress]);

            RuleFor(x => x.State)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredState])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxState]);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredCity])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxCity]);

            RuleFor(x => x.ZipCode)
               .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredZipCode])
               .Matches("^[0-9]+$").WithMessage(localizer[Constants.Resources.NumbersOnlyZipCode])
               .MaximumLength(10).WithMessage(localizer[Constants.Resources.MaxZipCode]);
        }
    }
}