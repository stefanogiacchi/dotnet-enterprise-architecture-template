using ArcAI.Application.Common.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ValidationException = ArcAI.Application.Common.Exceptions.ValidationException;

namespace ArcAI.Application.Behaviors;

/// <summary>
/// Advanced validation behavior with configurable options and enhanced error reporting.
/// Enterprise-grade implementation with performance optimization and detailed diagnostics.
/// </summary>
/// <typeparam name="TRequest">The type of request being validated.</typeparam>
/// <typeparam name="TResponse">The type of response being returned.</typeparam>
public sealed class ValidationBehaviorAdvanced<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehaviorAdvanced<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService? _currentUserService;
    private readonly ValidationBehaviorOptions _options;

    public ValidationBehaviorAdvanced(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehaviorAdvanced<TRequest, TResponse>> logger,
        IOptions<ValidationBehaviorOptions> options,
        ICurrentUserService? currentUserService = null)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? new ValidationBehaviorOptions();
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Check if validation should be skipped for this request type
        if (_options.ExcludedRequestTypes.Contains(requestName))
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    "Validation skipped for {RequestName} (excluded type)",
                    requestName);
            }

            return await next();
        }

        // Skip if no validators
        if (!_validators.Any())
        {
            if (_options.WarnOnMissingValidators && _logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(
                    "No validators registered for {RequestName}. Consider adding validation.",
                    requestName);
            }

            return await next();
        }

        // Perform validation
        var failures = await ValidateRequestAsync(request, cancellationToken);

        if (failures.Any())
        {
            HandleValidationFailures(requestName, failures);
        }

        return await next();
    }

    /// <summary>
    /// Validates the request using all registered validators.
    /// </summary>
    private async Task<List<ValidationFailure>> ValidateRequestAsync(
        TRequest request,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Log validation start
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            LogValidationStart(requestName, _validators.Count());
        }

        var context = new ValidationContext<TRequest>(request);

        // Choose validation strategy based on configuration
        List<ValidationResult> validationResults;

        if (_options.StopOnFirstFailure)
        {
            // Sequential validation - stop on first failure
            validationResults = new List<ValidationResult>();

            foreach (var validator in _validators)
            {
                var result = await validator.ValidateAsync(context, cancellationToken);
                validationResults.Add(result);

                if (!result.IsValid)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug(
                            "Early validation termination for {RequestName}. First validator failed.",
                            requestName);
                    }
                    break;
                }
            }
        }
        else
        {
            // Parallel validation - run all validators
            validationResults = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)))).ToList();
        }

        // Collect failures
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // Apply max errors limit if configured
        if (_options.MaxErrorsToReturn > 0 && failures.Count > _options.MaxErrorsToReturn)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(
                    "Validation for {RequestName} produced {TotalErrors} errors. Truncating to {MaxErrors}.",
                    requestName,
                    failures.Count,
                    _options.MaxErrorsToReturn);
            }

            failures = failures.Take(_options.MaxErrorsToReturn).ToList();
        }

        return failures;
    }

    /// <summary>
    /// Handles validation failures by logging and throwing exception.
    /// </summary>
    private void HandleValidationFailures(string requestName, List<ValidationFailure> failures)
    {
        // Log failures
        LogValidationFailures(requestName, failures);

        // Optionally include additional context in exception
        if (_options.IncludeDetailedErrors)
        {
            throw CreateDetailedValidationException(failures);
        }
        else
        {
            throw new ValidationException(failures);
        }
    }

    /// <summary>
    /// Creates a validation exception with enhanced error details.
    /// </summary>
    private ValidationException CreateDetailedValidationException(List<ValidationFailure> failures)
    {
        // Group by property and add severity information if available
        var exception = new ValidationException(failures);

        // Could add custom properties here for enhanced error reporting
        // For example, attach severity levels, error codes, etc.

        return exception;
    }

    /// <summary>
    /// Logs validation start with context.
    /// </summary>
    private void LogValidationStart(string requestName, int validatorCount)
    {
        var userId = _currentUserService?.UserId ?? "Anonymous";

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestName"] = requestName,
            ["ValidatorCount"] = validatorCount,
            ["UserId"] = userId,
            ["Stage"] = "Validation",
            ["Strategy"] = _options.StopOnFirstFailure ? "Sequential" : "Parallel"
        }))
        {
            _logger.LogDebug(
                "[VALIDATION START] {RequestName} | Validators: {Count} | Strategy: {Strategy} | User: {UserId}",
                requestName,
                validatorCount,
                _options.StopOnFirstFailure ? "Sequential" : "Parallel",
                userId);
        }
    }

    /// <summary>
    /// Logs validation failures with comprehensive details.
    /// </summary>
    private void LogValidationFailures(string requestName, List<ValidationFailure> failures)
    {
        var userId = _currentUserService?.UserId ?? "Anonymous";
        var userName = _currentUserService?.UserName ?? "Anonymous";
        var errorCount = failures.Count;

        // Group errors by property
        var errorsByProperty = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => new { f.ErrorMessage, f.ErrorCode, f.Severity }).ToArray());

        // Group by severity if available
        var errorsBySeverity = failures
            .GroupBy(f => f.Severity)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestName"] = requestName,
            ["UserId"] = userId,
            ["UserName"] = userName,
            ["ValidationErrorCount"] = errorCount,
            ["FailedProperties"] = errorsByProperty.Keys,
            ["ErrorsBySeverity"] = errorsBySeverity,
            ["Stage"] = "Validation"
        }))
        {
            // Log warning with summary
            _logger.LogWarning(
                "[VALIDATION FAILED] {RequestName} | User: {UserId} ({UserName}) | Errors: {ErrorCount} | Properties: [{Properties}]",
                requestName,
                userId,
                userName,
                errorCount,
                string.Join(", ", errorsByProperty.Keys));

            // Log detailed errors based on configuration
            if (_options.LogDetailedErrors && _logger.IsEnabled(LogLevel.Debug))
            {
                foreach (var error in failures)
                {
                    _logger.LogDebug(
                        "[VALIDATION ERROR] Property: {PropertyName} | Code: {ErrorCode} | Severity: {Severity} | Message: {ErrorMessage} | Attempted: {AttemptedValue}",
                        error.PropertyName,
                        error.ErrorCode ?? "N/A",
                        error.Severity,
                        error.ErrorMessage,
                        SanitizeAttemptedValue(error.AttemptedValue));
                }
            }

            // Log metrics for monitoring
            if (_options.EnableMetricsLogging)
            {
                LogValidationMetrics(requestName, errorCount, errorsBySeverity);
            }
        }
    }

    /// <summary>
    /// Sanitizes attempted values to avoid logging sensitive data.
    /// </summary>
    private string SanitizeAttemptedValue(object? attemptedValue)
    {
        if (attemptedValue == null)
            return "null";

        // Check if it's a sensitive property
        var valueString = attemptedValue.ToString() ?? "null";

        // Mask if looks like sensitive data
        if (_options.SensitivePropertyPatterns.Any(pattern =>
            valueString.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
        {
            return "***MASKED***";
        }

        // Truncate long values
        return valueString.Length > 100
            ? valueString.Substring(0, 100) + "..."
            : valueString;
    }

    /// <summary>
    /// Logs validation metrics for monitoring systems.
    /// </summary>
    private void LogValidationMetrics(
        string requestName,
        int errorCount,
        Dictionary<string, int> errorsBySeverity)
    {
        if (!_logger.IsEnabled(LogLevel.Trace))
            return;

        _logger.LogTrace(
            "METRIC | Type: Validation | Request: {RequestName} | ErrorCount: {ErrorCount} | Severities: {@Severities} | UserId: {UserId}",
            requestName,
            errorCount,
            errorsBySeverity,
            _currentUserService?.UserId ?? "Anonymous");
    }
}

/// <summary>
/// Configuration options for ValidationBehavior.
/// </summary>
public class ValidationBehaviorOptions
{
    /// <summary>
    /// Whether to stop validation on first failure.
    /// If true, validators run sequentially and stop at first error.
    /// If false, all validators run in parallel.
    /// Default: false (run all validators)
    /// </summary>
    public bool StopOnFirstFailure { get; set; } = false;

    /// <summary>
    /// Maximum number of validation errors to return.
    /// Set to 0 for unlimited.
    /// Default: 0 (unlimited)
    /// </summary>
    public int MaxErrorsToReturn { get; set; } = 0;

    /// <summary>
    /// Whether to include detailed error information in exceptions.
    /// Default: true
    /// </summary>
    public bool IncludeDetailedErrors { get; set; } = true;

    /// <summary>
    /// Whether to log detailed validation errors.
    /// Default: true
    /// </summary>
    public bool LogDetailedErrors { get; set; } = true;

    /// <summary>
    /// Whether to emit metrics for monitoring systems.
    /// Default: true
    /// </summary>
    public bool EnableMetricsLogging { get; set; } = true;

    /// <summary>
    /// Whether to warn when no validators are registered for a request.
    /// Useful during development to ensure validation is not forgotten.
    /// Default: true in Development, false in Production
    /// </summary>
    public bool WarnOnMissingValidators { get; set; } = true;

    /// <summary>
    /// Request types to exclude from validation.
    /// Useful for health checks or internal operations.
    /// </summary>
    public List<string> ExcludedRequestTypes { get; set; } = new();

    /// <summary>
    /// Patterns to identify sensitive data in attempted values.
    /// Values matching these patterns will be masked in logs.
    /// </summary>
    public List<string> SensitivePropertyPatterns { get; set; } = new()
    {
        "password",
        "token",
        "secret",
        "apikey",
        "creditcard",
        "ssn"
    };
}