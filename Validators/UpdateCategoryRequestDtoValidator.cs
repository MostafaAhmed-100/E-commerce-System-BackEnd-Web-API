using FluentValidation;
using WebApplication1.DTOS.Request_DTOs.Category;

namespace WebApplication1.Validators
{
    public class UpdateCategoryRequestDtoValidator : AbstractValidator<UpdateCategoryRequestDto>
    {
        public UpdateCategoryRequestDtoValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

            RuleFor(x => x.ParentCategoryId)
                .GreaterThan(0).WithMessage("Parent Category ID must be greater than 0.");
        }
    }
}