using ArcAI.Application.Common.Interfaces;

namespace ArcAI.Infrastructure.Services;

/// <summary>
/// Implementation of date and time service.
/// Provides abstraction over DateTime for testability.
/// </summary>
public class DateTimeService : IDateTimeService, IDateTime
{
    /// <inheritdoc/>
    public DateTime UtcNow => DateTime.UtcNow;

    /// <inheritdoc/>
    public DateTime Now => DateTime.Now;

    /// <inheritdoc/>
    public DateTime UtcToday => DateTime.UtcNow.Date;

    /// <inheritdoc/>
    public DateTime Today => DateTime.Today;

    /// <inheritdoc/>
    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;

    /// <inheritdoc/>
    public DateTimeOffset NowOffset => DateTimeOffset.Now;

    /// <inheritdoc/>
    public long UnixTimestamp => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    /// <inheritdoc/>
    public DateTime ToLocalTime(DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("DateTime must be in UTC", nameof(utcDateTime));
        }

        return utcDateTime.ToLocalTime();
    }

    /// <inheritdoc/>
    public DateTime ToUtcTime(DateTime localDateTime)
    {
        if (localDateTime.Kind == DateTimeKind.Utc)
        {
            return localDateTime;
        }

        return localDateTime.ToUniversalTime();
    }
}