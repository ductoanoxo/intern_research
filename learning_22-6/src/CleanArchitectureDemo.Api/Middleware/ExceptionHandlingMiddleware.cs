using FluentValidation;
using System.Text.Json;

namespace CleanArchitectureDemo.Api.Middleware;

/// <summary>
/// Custom Middleware for global exception handling.
/// Translates domain/application exceptions (e.g. ValidationException) into clear HTTP responses.
/// </summary>
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
            _logger.LogError(ex, "Exception caught in middleware: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var validationResult = JsonSerializer.Serialize(new
            {
                Title = "Validation Failed",
                Status = StatusCodes.Status400BadRequest,
                Errors = errors
            });

            return context.Response.WriteAsync(validationResult);
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        var result = JsonSerializer.Serialize(new
        {
            Title = "An unexpected error occurred.",
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message
        });

        return context.Response.WriteAsync(result);
    }
}
