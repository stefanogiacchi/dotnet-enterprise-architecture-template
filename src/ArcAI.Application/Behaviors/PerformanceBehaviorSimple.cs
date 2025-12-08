using ArcAI.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ArcAI.Application.Behaviors;

/// <summary>
/// Simple performance monitoring behavior with configurable threshold.
/// Improved version with proper stopwatch handling and user context.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
/// <typeparam name="TResponse">The type of response being returned.</typeparam>
public sealed class PerformanceBehaviorSimple<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehaviorSimple<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService? _currentUserService;

    /// <summary>
    /// Default warning threshold in milliseconds.
    /// Can be overridden via configuration.
    /// </summary>
    private readonly long _warningThresholdMs;

    public PerformanceBehaviorSimple(
        ILogger<PerformanceBehaviorSimple<TRequest, TResponse>> logger,
        ICurrentUserService? currentUserService = null,
        long warningThresholdMs = 500)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _currentUserService = currentUserService;
        _warningThresholdMs = warningThresholdMs;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Use Stopwatch for accurate timing
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Execute the request
            return await next();
        }
        finally
        {
            // Always stop the timer in finally block
            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            // Log if exceeds threshold
            if (elapsedMilliseconds > _warningThresholdMs)
            {
                LogSlowRequest(elapsedMilliseconds);
            }
            else if (_logger.IsEnabled(LogLevel.Debug))
            {
                // Log all requests in debug mode
                LogDebugPerformance(elapsedMilliseconds);
            }
        }
    }

    private void LogSlowRequest(long elapsedMilliseconds)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService?.UserId ?? "Anonymous";

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestName"] = requestName,
            ["ExecutionTimeMs"] = elapsedMilliseconds,
            ["ThresholdMs"] = _warningThresholdMs,
            ["UserId"] = userId
        }))
        {
            _logger.LogWarning(
                "Slow request detected: {RequestName} took {ElapsedMilliseconds}ms (threshold: {Threshold}ms) | User: {UserId}",
                requestName,
                elapsedMilliseconds,
                _warningThresholdMs,
                userId);
        }
    }

    private void LogDebugPerformance(long elapsedMilliseconds)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogDebug(
            "Request {RequestName} completed in {ElapsedMilliseconds}ms",
            requestName,
            elapsedMilliseconds);
    }
}