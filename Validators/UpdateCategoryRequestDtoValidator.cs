using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Request_DTOs.Category;

namespace WebApplication1.Validators
{
    public class UpdateCategoryRequestDtoValidator : AbstractValidator<UpdateCategoryRequestDto>
    {
        public UpdateCategoryRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage(localizer[Constants.Resources.RequiredCategoryName])
                .MaximumLength(100).WithMessage(localizer[Constants.Resources.MaxCategoryName]);

            RuleFor(x => x.ParentCategoryId)
                .GreaterThan(0).WithMessage(localizer[Constants.Resources.InvalidParentCategoryId]);
        }
    }
}