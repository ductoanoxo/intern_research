using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DddCqrsJwtDemo.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, response) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                (object)new
                {
                    Title = "Validation Failed",
                    Status = StatusCodes.Status400BadRequest,
                    Errors = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        )
                }),

            ArgumentException argumentException => (
                StatusCodes.Status400BadRequest,
                (object)new
                {
                    Title = "Bad Request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = argumentException.Message
                }),

            InvalidOperationException invalidOperationException => (
                StatusCodes.Status400BadRequest,
                (object)new
                {
                    Title = "Business Rule Violation",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = invalidOperationException.Message
                }),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                (object)new
                {
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "You are not authorized to perform this operation."
                }),

            _ => (
                StatusCodes.Status500InternalServerError,
                (object)new
                {
                    Title = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred. Please try again later."
                })
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
