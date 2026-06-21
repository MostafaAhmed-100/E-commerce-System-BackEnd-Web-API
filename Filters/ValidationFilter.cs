using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using WebApplication1.Exceptions;

namespace WebApplication1.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values.Where(v => v != null))
            {
                var argumentType = argument.GetType();

                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(argument);
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        var errors =  validationResult.Errors.Select(e => e.ErrorMessage);
                        throw new Exceptions.ValidationException(errors);
                    }
                }
            }

            await next();
        }
    }
}