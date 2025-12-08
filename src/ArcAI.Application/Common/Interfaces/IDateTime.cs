namespace ArcAI.Application.Common.Interfaces;

/// <summary>
/// Abstraction for date and time operations.
/// Enables testability by allowing time to be mocked in unit tests.
/// </summary>
public interface IDateTime
{
    /// <summary>
    /// Gets the current date and time in UTC.
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Gets the current date and time in local time.
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    /// Gets today's date (without time component) in UTC.
    /// </summary>
    DateTime UtcToday { get; }

    /// <summary>
    /// Gets today's date (without time component) in local time.
    /// </summary>
    DateTime Today { get; }

    /// <summary>
    /// Gets the current date and time as DateTimeOffset in UTC.
    /// </summary>
    DateTimeOffset UtcNowOffset { get; }

    /// <summary>
    /// Gets the current date and time as DateTimeOffset in local time.
    /// </summary>
    DateTimeOffset NowOffset { get; }

    /// <summary>
    /// Gets the current Unix timestamp (seconds since 1970-01-01).
    /// </summary>
    long UnixTimestamp { get; }

    /// <summary>
    /// Converts a UTC DateTime to local time.
    /// </summary>
    /// <param name="utcDateTime">The UTC DateTime to convert.</param>
    /// <returns>The DateTime in local time.</returns>
    DateTime ToLocalTime(DateTime utcDateTime);

    /// <summary>
    /// Converts a local DateTime to UTC.
    /// </summary>
    /// <param name="localDateTime">The local DateTime to convert.</param>
    /// <returns>The DateTime in UTC.</returns>
    DateTime ToUtcTime(DateTime localDateTime);
}