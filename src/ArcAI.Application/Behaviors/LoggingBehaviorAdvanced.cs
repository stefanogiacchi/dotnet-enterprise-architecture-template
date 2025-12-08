using ArcAI.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ArcAI.Application.Behaviors;

/// <summary>
/// Advanced pipeline behavior with configurable logging options and sensitive data masking.
/// Enterprise-grade implementation with performance monitoring and security features.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
/// <typeparam name="TResponse">The type of response being returned.</typeparam>
public class LoggingBehaviorAdvanced<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehaviorAdvanced<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService? _currentUserService;
    private readonly IDateTime? _dateTime;
    private readonly LoggingBehaviorOptions _options;

    public LoggingBehaviorAdvanced(
        ILogger<LoggingBehaviorAdvanced<TRequest, TResponse>> logger,
        IOptions<LoggingBehaviorOptions> options,
        ICurrentUserService? currentUserService = null,
        IDateTime? dateTime = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? new LoggingBehaviorOptions();
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Check if this request type should be excluded from logging
        if (_options.ExcludedRequestTypes.Contains(requestName))
        {
            return await next();
        }

        var responseName = typeof(TResponse).Name;
        var correlationId = _options.IncludeCorrelationId ? Guid.NewGuid().ToString() : null;
        var startTime = _dateTime?.UtcNow ?? DateTime.UtcNow;

        var stopwatch = Stopwatch.StartNew();

        try
        {
            LogRequestStart(requestName, correlationId, request);
            var response = await next();
            stopwatch.Stop();
            LogRequestSuccess(requestName, responseName, correlationId, stopwatch.ElapsedMilliseconds, response);
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogRequestFailure(requestName, correlationId, stopwatch.ElapsedMilliseconds, ex);
            throw;
        }
    }

    private void LogRequestStart(string requestName, string? correlationId, TRequest request)
    {
        if (!_logger.IsEnabled(LogLevel.Information))
            return;

        var scopeData = BuildLogScope(requestName, correlationId);

        string? requestData = null;
        if (_options.LogRequestData || _logger.IsEnabled(LogLevel.Debug))
        {
            requestData = SerializeAndMaskSensitiveData(request);
        }

        using (_logger.BeginScope(scopeData))
        {
            if (requestData != null)
            {
                _logger.LogDebug(
                    "[START] {RequestName} | CorrelationId: {CorrelationId} | User: {UserId} | Data: {RequestData}",
                    requestName,
                    correlationId ?? "N/A",
                    _currentUserService?.UserId ?? "Anonymous",
                    requestData);
            }
            else
            {
                _logger.LogInformation(
                    "[START] {RequestName} | CorrelationId: {CorrelationId} | User: {UserId}",
                    requestName,
                    correlationId ?? "N/A",
                    _currentUserService?.UserId ?? "Anonymous");
            }
        }
    }

    private void LogRequestSuccess(
        string requestName,
        string responseName,
        string? correlationId,
        long executionTimeMs,
        TResponse response)
    {
        if (!_logger.IsEnabled(LogLevel.Information))
            return;

        var scopeData = BuildLogScope(requestName, correlationId);
        scopeData["ResponseName"] = responseName;
        scopeData["ExecutionTimeMs"] = executionTimeMs;
        scopeData["Status"] = "Success";

        // Add performance category
        scopeData["PerformanceCategory"] = GetPerformanceCategory(executionTimeMs);

        string? responseData = null;
        if (_options.LogResponseData || _logger.IsEnabled(LogLevel.Debug))
        {
            responseData = SerializeAndMaskSensitiveData(response);
        }

        using (_logger.BeginScope(scopeData))
        {
            // Log as warning if slow request
            if (executionTimeMs > _options.SlowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "[SLOW] {RequestName} completed in {ExecutionTimeMs}ms (threshold: {ThresholdMs}ms) | CorrelationId: {CorrelationId} | User: {UserId}",
                    requestName,
                    executionTimeMs,
                    _options.SlowRequestThresholdMs,
                    correlationId ?? "N/A",
                    _currentUserService?.UserId ?? "Anonymous");
            }
            else if (responseData != null)
            {
                _logger.LogDebug(
                    "[SUCCESS] {RequestName} completed in {ExecutionTimeMs}ms | CorrelationId: {CorrelationId} | User: {UserId} | Response: {ResponseData}",
                    requestName,
                    executionTimeMs,
                    correlationId ?? "N/A",
                    _currentUserService?.UserId ?? "Anonymous",
                    responseData);
            }
            else
            {
                _logger.LogInformation(
                    "[SUCCESS] {RequestName} completed in {ExecutionTimeMs}ms | CorrelationId: {CorrelationId} | User: {UserId}",
                    requestName,
                    executionTimeMs,
                    correlationId ?? "N/A",
                    _currentUserService?.UserId ?? "Anonymous");
            }
        }

        // Log metrics for monitoring systems
        LogPerformanceMetrics(requestName, executionTimeMs);
    }

    private void LogRequestFailure(
        string requestName,
        string? correlationId,
        long executionTimeMs,
        Exception exception)
    {
        var scopeData = BuildLogScope(requestName, correlationId);
        scopeData["ExecutionTimeMs"] = executionTimeMs;
        scopeData["Status"] = "Failed";
        scopeData["ExceptionType"] = exception.GetType().Name;
        scopeData["ExceptionMessage"] = exception.Message;

        using (_logger.BeginScope(scopeData))
        {
            _logger.LogError(
                exception,
                "[FAILED] {RequestName} failed after {ExecutionTimeMs}ms | CorrelationId: {CorrelationId} | User: {UserId} | Exception: {ExceptionType} | Message: {ExceptionMessage}",
                requestName,
                executionTimeMs,
                correlationId ?? "N/A",
                _currentUserService?.UserId ?? "Anonymous",
                exception.GetType().Name,
                exception.Message);
        }
    }

    private Dictionary<string, object> BuildLogScope(string requestName, string? correlationId)
    {
        var scope = new Dictionary<string, object>
        {
            ["RequestName"] = requestName,
            ["Timestamp"] = _dateTime?.UtcNow ?? DateTime.UtcNow
        };

        if (_options.IncludeCorrelationId && correlationId != null)
        {
            scope["CorrelationId"] = correlationId;
        }

        if (_options.IncludeUserContext && _currentUserService != null)
        {
            scope["UserId"] = _currentUserService.UserId ?? "Anonymous";
            scope["UserName"] = _currentUserService.UserName ?? "Anonymous";
            scope["IsAuthenticated"] = _currentUserService.IsAuthenticated;
        }

        return scope;
    }

    private string? SerializeAndMaskSensitiveData<T>(T data)
    {
        if (data == null)
            return null;

        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                MaxDepth = _options.JsonSerializationMaxDepth,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var json = JsonSerializer.Serialize(data, options);

            // Mask sensitive data
            json = MaskSensitiveData(json);

            // Truncate if needed
            if (json.Length > _options.MaxSerializedDataLength)
            {
                json = json.Substring(0, _options.MaxSerializedDataLength) + "... (truncated)";
            }

            return json;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to serialize data for logging");
            return data.ToString();
        }
    }

    private string MaskSensitiveData(string json)
    {
        if (string.IsNullOrEmpty(json))
            return json;

        // Mask sensitive property values using regex
        foreach (var sensitiveProperty in _options.SensitivePropertyNames)
        {
            // Match pattern: "propertyName":"value" or "propertyName":value
            var pattern = $"\"{sensitiveProperty}\"\\s*:\\s*\"([^\"]+)\"";
            json = Regex.Replace(
                json,
                pattern,
                $"\"{sensitiveProperty}\":\"***MASKED***\"",
                RegexOptions.IgnoreCase);

            // Also handle non-quoted values
            pattern = $"\"{sensitiveProperty}\"\\s*:\\s*([^,}}]+)";
            json = Regex.Replace(
                json,
                pattern,
                $"\"{sensitiveProperty}\":\"***MASKED***\"",
                RegexOptions.IgnoreCase);
        }

        return json;
    }

    private string GetPerformanceCategory(long executionTimeMs)
    {
        return executionTimeMs switch
        {
            < 100 => "Fast",
            < 500 => "Normal",
            < 1000 => "Acceptable",
            < 3000 => "Slow",
            _ => "VerySlow"
        };
    }

    private void LogPerformanceMetrics(string requestName, long executionTimeMs)
    {
        // This method can be extended to send metrics to monitoring systems
        // Examples: Application Insights, Prometheus, DataDog, etc.

        // For now, we'll just log structured data that can be picked up by log aggregators
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "Performance Metric | Request: {RequestName} | Duration: {DurationMs} | Category: {Category}",
                requestName,
                executionTimeMs,
                GetPerformanceCategory(executionTimeMs));
        }
    }
}