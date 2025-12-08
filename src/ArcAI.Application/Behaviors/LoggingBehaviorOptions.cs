namespace ArcAI.Application.Behaviors;

/// <summary>
/// Configuration options for LoggingBehavior.
/// Allows customization of logging behavior through application settings.
/// </summary>
public class LoggingBehaviorOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to log request data.
    /// Default: false (only enabled in Debug log level)
    /// </summary>
    public bool LogRequestData { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to log response data.
    /// Default: false (only enabled in Debug log level)
    /// </summary>
    public bool LogResponseData { get; set; } = false;

    /// <summary>
    /// Gets or sets the slow request threshold in milliseconds.
    /// Requests exceeding this threshold will be logged as warnings.
    /// Default: 1000ms (1 second)
    /// </summary>
    public int SlowRequestThresholdMs { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the maximum length for serialized request/response data.
    /// Data exceeding this length will be truncated.
    /// Default: 1000 characters
    /// </summary>
    public int MaxSerializedDataLength { get; set; } = 1000;

    /// <summary>
    /// Gets or sets a value indicating whether to include user context in logs.
    /// Default: true
    /// </summary>
    public bool IncludeUserContext { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to include correlation IDs.
    /// Default: true
    /// </summary>
    public bool IncludeCorrelationId { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum depth for JSON serialization.
    /// Prevents deep object graphs from causing issues.
    /// Default: 3
    /// </summary>
    public int JsonSerializationMaxDepth { get; set; } = 3;

    /// <summary>
    /// Gets or sets a list of request types to exclude from logging.
    /// Useful for health checks or high-volume operations.
    /// </summary>
    public List<string> ExcludedRequestTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets a list of property names to exclude from serialization.
    /// Useful for sensitive data like passwords, tokens, etc.
    /// </summary>
    public List<string> SensitivePropertyNames { get; set; } = new()
    {
        "password",
        "token",
        "secret",
        "apikey",
        "authorization",
        "creditcard",
        "ssn"
    };
}