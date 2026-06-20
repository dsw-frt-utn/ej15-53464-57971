using Dsw2026Ej15.Domain.Exceptions;
using System.Text.Json;

namespace Dsw2026Ej15.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                
                await _next(context);
            }
            catch (Exception ex)
            {
                
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            
            if (exception is ValidationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var result = JsonSerializer.Serialize(new { error = exception.Message });
                return context.Response.WriteAsync(result);
            }

            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var problemDetails = JsonSerializer.Serialize(new { error = "Internal Server Error", details = exception.Message });
            return context.Response.WriteAsync(problemDetails);
        }
    }
}