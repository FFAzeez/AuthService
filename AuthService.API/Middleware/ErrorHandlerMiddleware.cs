using FluentValidation;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthService.API.Middleware
{
    public class ErrorHandlerMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context)
        {
            Log.Information("ErrorHandlerMiddleWare invoked");
            try
            {
                await next(context);
            }
            catch (ValidationException vex)
            {
                Log.Warning(vex, "Validation failed");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    IsSuccessful = false,
                    message = vex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    IsSuccessful = false,
                    message = "An unexpected error occurred",
                });
            }
        }
    }

}
