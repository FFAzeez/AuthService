using AuthService.Application.DTOs.Requests;
using AuthService.Application.Validators;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthService.API.Filters
{
    public class RegisterUserValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Log.Information($"validate register params");

            // Extract the DTO that MVC model binding already populated
            if (context.ActionArguments.TryGetValue("request", out var dto) && dto is RegisterDto register)
            {
                var validator = new RegisterUserValidator();
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
