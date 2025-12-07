namespace ArcAI.Domain.Common;

/// <summary>
/// Marker interface for domain events.
/// Domain events represent significant occurrences within the domain
/// that other parts of the system may need to react to.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the unique identifier for this event instance.
    /// </summary>
    Guid EventId { get; }
}

/// <summary>
/// Base implementation for domain events providing common properties.
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the unique identifier for this event instance.
    /// </summary>
    public Guid EventId { get; } = Guid.NewGuid();
}