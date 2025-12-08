using ArcAI.Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ValidationException = ArcAI.Application.Common.Exceptions.ValidationException;

namespace ArcAI.Application.Behaviors;

/// <summary>
/// Pipeline behavior for validating requests using FluentValidation.
/// Provides comprehensive validation with detailed error reporting and logging.
/// </summary>
/// <typeparam name="TRequest">The type of request being validated.</typeparam>
/// <typeparam name="TResponse">The type of response being returned.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService? _currentUserService;

    /// <summary>
    /// Initializes a new instance of the ValidationBehavior class.
    /// </summary>
    /// <param name="validators">Collection of validators for this request type.</param>
    /// <param name="logger">Logger instance for validation tracking.</param>
    /// <param name="currentUserService">Optional current user service for context enrichment.</param>
    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger,
        ICurrentUserService? currentUserService = null)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Handles the request validation pipeline.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Skip validation if no validators registered
        if (!_validators.Any())
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    "No validators registered for {RequestName}. Skipping validation.",
                    requestName);
            }

            return await next();
        }

        // Log validation start
        LogValidationStart(requestName);

        // Create validation context
        var context = new ValidationContext<TRequest>(request);

        // Execute all validators in parallel
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Collect all validation failures
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null);

        // If validation failed, throw exception with detailed errors
        if (failures.Any())
        {
            var failureList = failures.ToList();
            LogValidationFailure(requestName, failureList);
            throw new ValidationException(failures); // ✅ Passa IEnumerable direttamente
        }

        // Log successful validation
        LogValidationSuccess(requestName);

        // Proceed to next behavior in pipeline
        return await next();
    }

    /// <summary>
    /// Logs the start of validation.
    /// </summary>
    private void LogValidationStart(string requestName)
    {
        if (!_logger.IsEnabled(LogLevel.Debug))
            return;

        var validatorCount = _validators.Count();
        var userId = _currentUserService?.UserId ?? "Anonymous";

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestName"] = requestName,
            ["ValidatorCount"] = validatorCount,
            ["UserId"] = userId,
            ["Stage"] = "Validation"
        }))
        {
            _logger.LogDebug(
                "Validating {RequestName} with {ValidatorCount} validator(s) | User: {UserId}",
                requestName,
                validatorCount,
                userId);
        }
    }

    /// <summary>
    /// Logs successful validation.
    /// </summary>
    private void LogValidationSuccess(string requestName)
    {
        if (!_logger.IsEnabled(LogLevel.Debug))
            return;

        var userId = _currentUserService?.UserId ?? "Anonymous";

        _logger.LogDebug(
            "Validation passed for {RequestName} | User: {UserId}",
            requestName,
            userId);
    }

    /// <summary>
    /// Logs validation failures with detailed error information.
    /// </summary>
    private void LogValidationFailure(string requestName, List<FluentValidation.Results.ValidationFailure> failures)
    {
        var userId = _currentUserService?.UserId ?? "Anonymous";
        var userName = _currentUserService?.UserName ?? "Anonymous";
        var errorCount = failures.Count;

        // Group errors by property for structured logging
        var errorsByProperty = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray());

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestName"] = requestName,
            ["UserId"] = userId,
            ["UserName"] = userName,
            ["ValidationErrorCount"] = errorCount,
            ["FailedProperties"] = errorsByProperty.Keys,
            ["Stage"] = "Validation"
        }))
        {
            // Log warning with summary
            _logger.LogWarning(
                "Validation failed for {RequestName} | User: {UserId} | Errors: {ErrorCount} | Properties: {Properties}",
                requestName,
                userId,
                errorCount,
                string.Join(", ", errorsByProperty.Keys));

            // Log detailed errors in debug mode
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                foreach (var error in failures)
                {
                    _logger.LogDebug(
                        "Validation error | Property: {PropertyName} | Error: {ErrorMessage} | AttemptedValue: {AttemptedValue}",
                        error.PropertyName,
                        error.ErrorMessage,
                        error.AttemptedValue ?? "null");
                }
            }
        }
    }
}