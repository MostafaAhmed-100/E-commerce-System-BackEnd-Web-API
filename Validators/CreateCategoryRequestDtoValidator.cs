using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class CreateCategoryRequestDtoValidator : AbstractValidator<CreateCategoryRequestDto>
    {
        public CreateCategoryRequestDtoValidator() 
        {
            RuleFor(c => c.CategoryName)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

            RuleFor(p => p.ParentCategoryId)
                .GreaterThan(0).WithMessage("Parent Category ID must be greater than 0.");
        } 
    }
}
