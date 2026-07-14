using FluentValidation;
using WebApplication1.DTOS.Request_DTOs;

namespace WebApplication1.Validators
{
    public class RenameWishlistDtoValidators : AbstractValidator<RenameWishlistDto>
    {
        public RenameWishlistDtoValidators() 
        {
            RuleFor(x => x.NewWishlistName)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
