namespace ArcAI.Domain.Common;

/// <summary>
/// Base class for aggregate roots with identity.
/// Aggregates define consistency boundaries and are the unit of persistence.
/// </summary>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : notnull
{
    /// <summary>
    /// Gets or sets the version number for optimistic concurrency control.
    /// </summary>
    public int Version { get; protected set; }

    protected AggregateRoot(TId id) : base(id) { }

    protected AggregateRoot() : base() { }

    /// <summary>
    /// Increments the version number when the aggregate is modified.
    /// </summary>
    protected void IncrementVersion()
    {
        Version++;
    }
}

/// <summary>
/// Simplified aggregate root base class using Guid as identifier.
/// </summary>
public abstract class AggregateRoot : AggregateRoot<Guid>
{
    protected AggregateRoot(Guid id) : base(id) { }
    protected AggregateRoot() : base() { }
}