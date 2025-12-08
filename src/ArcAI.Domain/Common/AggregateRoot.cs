namespace ArcAI.Domain.Common;

/// <summary>
/// Base class for aggregate roots with identity and domain events support.
/// Aggregates define consistency boundaries and are the unit of persistence.
/// </summary>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the domain events pending dispatch.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Gets or sets the version number for optimistic concurrency control.
    /// </summary>
    public int Version { get; protected set; }

    /// <summary>
    /// Initializes a new instance with the specified identifier.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    protected AggregateRoot(TId id) : base(id)
    {
        Version = 0;
    }

    /// <summary>
    /// Parameterless constructor for ORM support.
    /// </summary>
    protected AggregateRoot() : base()
    {
        Version = 0;
    }

    /// <summary>
    /// Adds a domain event to the collection of pending events.
    /// Events will be dispatched after successful persistence.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent == null)
            throw new ArgumentNullException(nameof(domainEvent));

        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a specific domain event from the pending events collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent == null)
            throw new ArgumentNullException(nameof(domainEvent));

        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all pending domain events after dispatch.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Increments the version number when the aggregate is modified.
    /// Used for optimistic concurrency control.
    /// </summary>
    protected void IncrementVersion()
    {
        Version++;
    }
}

/// <summary>
/// Simplified aggregate root base class using Guid as identifier.
/// Most aggregates in the system will inherit from this class.
/// </summary>
public abstract class AggregateRoot : AggregateRoot<Guid>
{
    /// <summary>
    /// Initializes a new instance with the specified Guid identifier.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    protected AggregateRoot(Guid id) : base(id)
    {
    }

    /// <summary>
    /// Parameterless constructor for ORM support.
    /// </summary>
    protected AggregateRoot() : base()
    {
    }

    /// <summary>
    /// Creates a new aggregate with a new Guid identifier.
    /// </summary>
    protected AggregateRoot(bool generateNewId) : base(generateNewId ? Guid.NewGuid() : Guid.Empty)
    {
    }
}