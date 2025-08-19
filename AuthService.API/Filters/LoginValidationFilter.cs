using AuthService.Application.DTOs.Requests;
using AuthService.Application.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace AuthService.API.Filters
{
    public class LoginValidationFilter: IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Log.Information($"validate login params");

            if (context.ActionArguments.TryGetValue("request", out var dto) && dto is LoginRequestDto register)
            {
                var validator = new LoginRequestValidator();
                var validationResult = await validator.ValidateAsync(register);

                if (!validationResult.IsValid)
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        IsSuccessful = false,
                        message = string.Join(",", validationResult.Errors),
                    });
                    return;
                }
            }

            await next();
        }
    }
}
