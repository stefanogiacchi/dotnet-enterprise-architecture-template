namespace ArcAI.Domain.Common;

/// <summary>
/// Marker interface for aggregate roots.
/// Aggregate roots are the entry point for accessing and modifying
/// a cluster of related domain objects (an aggregate).
/// Only aggregate roots should be directly persisted by repositories.
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Gets the domain events pending dispatch.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears all pending domain events after dispatch.
    /// </summary>
    void ClearDomainEvents();
}