namespace ArcAI.Application.Common.Interfaces;

/// <summary>
/// Service interface for date and time operations.
/// Alternative naming to IDateTime for service-based implementations.
/// </summary>
public interface IDateTimeService
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
    /// Gets today's date in UTC.
    /// </summary>
    DateTime UtcToday { get; }

    /// <summary>
    /// Gets today's date in local time.
    /// </summary>
    DateTime Today { get; }
}