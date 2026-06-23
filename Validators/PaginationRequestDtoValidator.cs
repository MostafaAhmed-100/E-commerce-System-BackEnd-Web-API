using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApplication1.DTOS.Shared.RequestDto;

namespace WebApplication1.Validators
{
    public class PaginationRequestDtoValidator : AbstractValidator<PaginationRequestDto>
    {
        public PaginationRequestDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage(localizer[Constants.Resources.InvalidPageNumber]);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50).WithMessage(localizer[Constants.Resources.InvalidPageSize]);
        }
    }
}