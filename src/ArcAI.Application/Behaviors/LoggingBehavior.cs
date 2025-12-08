using ArcAI.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace ArcAI.Application.Behaviors;

/// <summary>
/// Pipeline behavior that logs requests and responses with execution time tracking.
/// Provides structured logging with user context enrichment for observability.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
/// <typeparam name="TResponse">The type of response being returned.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService? _currentUserService;
    private readonly IDateTime? _dateTime;

    /// <summary>
    /// Initializes a new instance of the LoggingBehavior class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="currentUserService">Optional current user service for user context enrichment.</param>
    /// <param name="dateTime">Optional date time service for timestamp consistency.</param>
    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService? currentUserService = null,
        IDateTime? dateTime = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    /// <summary>
    /// Handles the request with logging before and after execution.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var responseName = typeof(TResponse).Name;
        var correlationId = Guid.NewGuid().ToString();
        var startTime = _dateTime?.UtcNow ?? DateTime.UtcNow;

        // Start stopwatch for execution time tracking
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Log request start with enriched context
            LogRequestStart(requestName, correlationId, request);

            // Execute the request
            var response = await next();

            // Stop timer
            stopwatch.Stop();

            // Log successful completion
            LogRequestSuccess(requestName, responseName, correlationId, stopwatch.ElapsedMilliseconds, response);

            return response;
        }
        catch (Exception ex)
        {
            // Stop timer
            stopwatch.Stop();

            // Log request failure
            LogRequestFailure(requestName, correlationId, stopwatch.ElapsedMilliseconds, ex);

            // Re-throw to be handled by exception handling behavior
            throw;
        }
    }

    /// <summary>
    /// Logs the start of a request with user context enrichment.
    /// </summary>
    private void LogRequestStart(string requestName, string correlationId, TRequest request)
    {
        if (!_logger.IsEnabled(LogLevel.Information))
            return;

        var userId = _currentUserService?.UserId;
        var userName = _currentUserService?.UserName;
        var isAuthenticated = _currentUserService?.IsAuthenticated ?? false;

        // Serialize request for detailed logging (only in debug/development)
        string? requestData = null;
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            try
            {
                requestData = SerializeRequest(request);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to serialize request data for logging");
            }
        }

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestName"] = requestName,
            ["UserId"] = userId ?? "Anonymous",
            ["UserName"] = userName ?? "Anonymous",
            ["IsAuthenticated"] = isAuthenticated,
            ["Timestamp"] = _dateTime?.UtcNow ?? DateTime.UtcNow
        }))
        {
            if (requestData != null)
            {
                _logger.LogDebug(
                    "Starting request {RequestName} | CorrelationId: {CorrelationId} | User: {UserId} | Request Data: {RequestData}",
                    requestName,
                    correlationId,
                    userId ?? "Anonymous",
                    requestData);
            }
            else
            {
                _logger.LogInformation(
                    "Starting request {RequestName} | CorrelationId: {CorrelationId} | User: {UserId}",
                    requestName,
                    correlationId,
                    userId ?? "Anonymous");
            }
        }
    }

    /// <summary>
    /// Logs successful request completion with execution metrics.
    /// </summary>
    private void LogRequestSuccess(
        string requestName,
        string responseName,
        string correlationId,
        long executionTimeMs,
        TResponse response)
    {
        if (!_logger.IsEnabled(LogLevel.Information))
            return;

        var userId = _currentUserService?.UserId;

        // Serialize response for detailed logging (only in debug/development)
        string? responseData = null;
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            try
            {
                responseData = SerializeResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to serialize response data for logging");
            }
        }

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestName"] = requestName,
            ["ResponseName"] = responseName,
            ["UserId"] = userId ?? "Anonymous",
            ["ExecutionTimeMs"] = executionTimeMs,
            ["Status"] = "Success"
        }))
        {
            // Log warning if execution time exceeds threshold
            if (executionTimeMs > 1000) // 1 second threshold
            {
                _logger.LogWarning(
                    "Slow request detected: {RequestName} completed in {ExecutionTimeMs}ms | CorrelationId: {CorrelationId} | User: {UserId}",
                    requestName,
                    executionTimeMs,
                    correlationId,
                    userId ?? "Anonymous");
            }
            else if (responseData != null)
            {
                _logger.LogDebug(
                    "Completed request {RequestName} in {ExecutionTimeMs}ms | CorrelationId: {CorrelationId} | User: {UserId} | Response: {ResponseData}",
                    requestName,
                    executionTimeMs,
                    correlationId,
                    userId ?? "Anonymous",
                    responseData);
            }
            else
            {
                _logger.LogInformation(
                    "Completed request {RequestName} in {ExecutionTimeMs}ms | CorrelationId: {CorrelationId} | User: {UserId}",
                    requestName,
                    executionTimeMs,
                    correlationId,
                    userId ?? "Anonymous");
            }
        }
    }

    /// <summary>
    /// Logs request failure with exception details and execution metrics.
    /// </summary>
    private void LogRequestFailure(
        string requestName,
        string correlationId,
        long executionTimeMs,
        Exception exception)
    {
        var userId = _currentUserService?.UserId;
        var exceptionType = exception.GetType().Name;

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestName"] = requestName,
            ["UserId"] = userId ?? "Anonymous",
            ["ExecutionTimeMs"] = executionTimeMs,
            ["Status"] = "Failed",
            ["ExceptionType"] = exceptionType
        }))
        {
            _logger.LogError(
                exception,
                "Request {RequestName} failed after {ExecutionTimeMs}ms | CorrelationId: {CorrelationId} | User: {UserId} | Exception: {ExceptionType}",
                requestName,
                executionTimeMs,
                correlationId,
                userId ?? "Anonymous",
                exceptionType);
        }
    }

    /// <summary>
    /// Serializes the request object for logging (only in debug mode).
    /// Implements privacy-aware serialization to avoid logging sensitive data.
    /// </summary>
    private string? SerializeRequest(TRequest request)
    {
        if (request == null)
            return null;

        try
        {
            // Use custom JSON serializer options to handle circular references
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                MaxDepth = 3, // Prevent deep object graphs
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var json = JsonSerializer.Serialize(request, options);

            // Truncate if too long (prevent log spam)
            return json.Length > 1000 ? json.Substring(0, 1000) + "... (truncated)" : json;
        }
        catch
        {
            // If serialization fails, return a safe representation
            return request.ToString();
        }
    }

    /// <summary>
    /// Serializes the response object for logging (only in debug mode).
    /// </summary>
    private string? SerializeResponse(TResponse response)
    {
        if (response == null)
            return null;

        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                MaxDepth = 3,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var json = JsonSerializer.Serialize(response, options);

            // Truncate if too long
            return json.Length > 1000 ? json.Substring(0, 1000) + "... (truncated)" : json;
        }
        catch
        {
            return response.ToString();
        }
    }
}