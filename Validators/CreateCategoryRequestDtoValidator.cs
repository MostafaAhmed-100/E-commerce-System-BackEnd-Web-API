using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs;
namespace WebApplication1.Validators
{
    public class CreateCategoryRequestDtoValidator : AbstractValidator<CreateCategoryRequestDto>
    {
        public CreateCategoryRequestDtoValidator(IStringLocalizer<SharedResource> localizer) 
        {
            RuleFor(c => c.CategoryName)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredCategoryName])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxCategoryName]);

            RuleFor(p => p.ParentCategoryId)
                .GreaterThan(0).WithMessage(localizer[Constants.Resources.InvalidParentCategoryId]);
        } 
    }
}
