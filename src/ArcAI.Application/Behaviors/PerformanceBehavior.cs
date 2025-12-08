using ArcAI.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ArcAI.Application.Behaviors;

/// <summary>
/// Pipeline behavior for monitoring and logging request performance.
/// Provides configurable thresholds, detailed metrics, and user context enrichment.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
/// <typeparam name="TResponse">The type of response being returned.</typeparam>
public sealed class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService? _currentUserService;
    private readonly PerformanceBehaviorOptions _options;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        IOptions<PerformanceBehaviorOptions> options,
        ICurrentUserService? currentUserService = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? new PerformanceBehaviorOptions();
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Check if this request type should be excluded from performance monitoring
        if (_options.ExcludedRequestTypes.Contains(requestName))
        {
            return await next();
        }

        // Get custom threshold for this request type if configured
        var threshold = GetThresholdForRequest(requestName);

        var stopwatch = Stopwatch.StartNew();
        TResponse response;

        try
        {
            response = await next();
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            // Log performance metrics
            LogPerformanceMetrics(requestName, elapsedMilliseconds, threshold);
        }

        return response;
    }

    /// <summary>
    /// Gets the performance threshold for a specific request type.
    /// Uses custom threshold if configured, otherwise falls back to default.
    /// </summary>
    private long GetThresholdForRequest(string requestName)
    {
        if (_options.CustomThresholds.TryGetValue(requestName, out var customThreshold))
        {
            return customThreshold;
        }

        return _options.DefaultWarningThresholdMs;
    }

    /// <summary>
    /// Logs performance metrics based on execution time and configured thresholds.
    /// </summary>
    private void LogPerformanceMetrics(string requestName, long elapsedMilliseconds, long threshold)
    {
        var performanceCategory = GetPerformanceCategory(elapsedMilliseconds);
        var userId = _currentUserService?.UserId ?? "Anonymous";
        var userName = _currentUserService?.UserName ?? "Anonymous";

        // Build structured log scope
        var logScope = new Dictionary<string, object>
        {
            ["RequestName"] = requestName,
            ["ExecutionTimeMs"] = elapsedMilliseconds,
            ["Threshold"] = threshold,
            ["PerformanceCategory"] = performanceCategory,
            ["UserId"] = userId,
            ["UserName"] = userName,
            ["Timestamp"] = DateTime.UtcNow
        };

        using (_logger.BeginScope(logScope))
        {
            // Determine log level based on performance category
            if (elapsedMilliseconds > _options.CriticalThresholdMs)
            {
                // CRITICAL: Very slow operation
                _logger.LogError(
                    "[CRITICAL PERFORMANCE] Request {RequestName} took {ElapsedMilliseconds}ms (threshold: {Threshold}ms, critical: {CriticalThreshold}ms) | User: {UserId} | Category: {Category}",
                    requestName,
                    elapsedMilliseconds,
                    threshold,
                    _options.CriticalThresholdMs,
                    userId,
                    performanceCategory);
            }
            else if (elapsedMilliseconds > threshold)
            {
                // WARNING: Slow operation
                _logger.LogWarning(
                    "[SLOW REQUEST] {RequestName} took {ElapsedMilliseconds}ms (threshold: {Threshold}ms) | User: {UserId} | Category: {Category}",
                    requestName,
                    elapsedMilliseconds,
                    threshold,
                    userId,
                    performanceCategory);
            }
            else if (_options.LogAllRequests && _logger.IsEnabled(LogLevel.Information))
            {
                // INFO: Normal performance
                _logger.LogInformation(
                    "[PERFORMANCE] {RequestName} completed in {ElapsedMilliseconds}ms | User: {UserId} | Category: {Category}",
                    requestName,
                    elapsedMilliseconds,
                    userId,
                    performanceCategory);
            }
            else if (_logger.IsEnabled(LogLevel.Debug))
            {
                // DEBUG: Detailed performance metrics
                _logger.LogDebug(
                    "[PERFORMANCE DEBUG] {RequestName} | Duration: {ElapsedMilliseconds}ms | Category: {Category} | User: {UserId}",
                    requestName,
                    elapsedMilliseconds,
                    performanceCategory,
                    userId);
            }
        }

        // Log metrics for external monitoring systems
        if (_options.EnableMetricsLogging)
        {
            LogMetricsForMonitoring(requestName, elapsedMilliseconds, performanceCategory);
        }
    }

    /// <summary>
    /// Categorizes request performance based on execution time.
    /// </summary>
    private string GetPerformanceCategory(long elapsedMilliseconds)
    {
        return elapsedMilliseconds switch
        {
            < 50 => "Excellent",      // < 50ms
            < 100 => "VeryFast",      // 50-100ms
            < 200 => "Fast",          // 100-200ms
            < 500 => "Normal",        // 200-500ms
            < 1000 => "Acceptable",   // 500ms-1s
            < 2000 => "Slow",         // 1-2s
            < 5000 => "VerySlow",     // 2-5s
            _ => "Critical"           // >5s
        };
    }

    /// <summary>
    /// Logs metrics in a format suitable for external monitoring systems.
    /// Can be picked up by Application Insights, Prometheus, DataDog, etc.
    /// </summary>
    private void LogMetricsForMonitoring(string requestName, long elapsedMilliseconds, string category)
    {
        if (!_logger.IsEnabled(LogLevel.Trace))
            return;

        // Structured metric log for monitoring systems
        _logger.LogTrace(
            "METRIC | Type: Performance | Request: {RequestName} | Duration: {DurationMs} | Category: {Category} | UserId: {UserId}",
            requestName,
            elapsedMilliseconds,
            category,
            _currentUserService?.UserId ?? "Anonymous");
    }
}

/// <summary>
/// Configuration options for PerformanceBehavior.
/// </summary>
public class PerformanceBehaviorOptions
{
    /// <summary>
    /// Default warning threshold in milliseconds.
    /// Requests exceeding this will generate a warning log.
    /// Default: 500ms
    /// </summary>
    public long DefaultWarningThresholdMs { get; set; } = 500;

    /// <summary>
    /// Critical threshold in milliseconds.
    /// Requests exceeding this will generate an error log.
    /// Default: 5000ms (5 seconds)
    /// </summary>
    public long CriticalThresholdMs { get; set; } = 5000;

    /// <summary>
    /// Custom thresholds per request type.
    /// Allows fine-grained control over performance expectations.
    /// Example: { "SearchProductsQuery": 1000, "GenerateReportCommand": 10000 }
    /// </summary>
    public Dictionary<string, long> CustomThresholds { get; set; } = new();

    /// <summary>
    /// Request types to exclude from performance monitoring.
    /// Useful for health checks or high-frequency operations.
    /// </summary>
    public List<string> ExcludedRequestTypes { get; set; } = new();

    /// <summary>
    /// Whether to log all requests regardless of performance.
    /// Default: false (only logs slow requests)
    /// </summary>
    public bool LogAllRequests { get; set; } = false;

    /// <summary>
    /// Whether to emit structured metrics for external monitoring systems.
    /// Default: true
    /// </summary>
    public bool EnableMetricsLogging { get; set; } = true;
}