using System.Collections.Concurrent;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NetWalker.API.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    private static readonly ConcurrentDictionary<Type, Type> ValidatorTypeCache = new();

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var argumentType = argument.GetType();
            
            var validatorType = ValidatorTypeCache.GetOrAdd(
                argumentType,
                type => typeof(IValidator<>).MakeGenericType(type)
            );
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator) 
                continue;

            var validationContext = new ValidationContext<object>(argument);
            var validationResult = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (!validationResult.IsValid)
            {

                foreach (var err in validationResult.Errors)
                {
                    context.ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
                }

                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed"
                });
                return;
            }
        }

        await next();
    }
}