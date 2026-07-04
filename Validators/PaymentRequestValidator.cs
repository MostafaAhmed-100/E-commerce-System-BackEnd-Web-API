using FluentValidation;
using WebApplication1.PaymentGateway.External.Request;

namespace WebApplication1.Validators
{
    public class PaymentRequestValidator : AbstractValidator<PaymentRequestDto>
    {
        public PaymentRequestValidator() 
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("the orderId cannot be empty")
                .GreaterThan(0).WithMessage("OrderId must be grater than 0");

            RuleFor(x => x.TotalAmount)
                .NotEmpty().WithMessage("the TotalAmount cannot be empty")
                .GreaterThan(0).WithMessage("TotalAmount must be grater than 0");

            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("the CustomerName cannot be empty")
                .Length(min: 3,max: 50).WithMessage("CustomerName must be between 3 and 50 ");

            RuleFor(x => x.CustomerEmail)
                .NotEmpty().WithMessage("the CustomerEmail cannot be empty")
                .EmailAddress();


        }
    }
}
