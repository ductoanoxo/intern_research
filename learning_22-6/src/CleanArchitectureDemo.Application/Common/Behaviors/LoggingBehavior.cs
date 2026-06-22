using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureDemo.Application.Common.Behaviors;

/// <summary>
/// MediatR Pipeline Behavior for logging all requests.
/// Demonstrates:
/// - Open/Closed Principle (OCP): We can extend request execution with logging behavior 
///   without modifying individual handlers.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("[MediatR] Handling request {RequestName}", requestName);

        var response = await next();

        _logger.LogInformation("[MediatR] Handled request {RequestName}", requestName);
        return response;
    }
}
