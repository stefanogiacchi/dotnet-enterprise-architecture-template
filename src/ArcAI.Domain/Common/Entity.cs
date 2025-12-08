namespace ArcAI.Domain.Common;

/// <summary>
/// Base class for entities with strongly-typed identity.
/// Entities have a unique identifier and are distinguished by their ID, not their attributes.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    /// <summary>
    /// Gets the entity's unique identifier.
    /// </summary>
    public TId Id { get; protected set; }

    /// <summary>
    /// Initializes a new instance with the specified identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    protected Entity(TId id)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id), "Entity ID cannot be null");

        Id = id;
    }

    /// <summary>
    /// Parameterless constructor for ORM support.
    /// Protected to prevent direct instantiation.
    /// </summary>
    protected Entity()
    {
        // Required by EF Core for materialization
        // Id will be set by the ORM
        Id = default!;
    }

    #region Equality

    /// <summary>
    /// Determines whether the specified entity is equal to the current entity.
    /// Two entities are equal if they have the same type and ID.
    /// </summary>
    public bool Equals(Entity<TId>? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current entity.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj is not Entity<TId> entity)
            return false;

        return Equals(entity);
    }

    /// <summary>
    /// Returns the hash code for this entity.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }

    #endregion

    /// <summary>
    /// Returns a string representation of the entity.
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name} [Id={Id}]";
    }
}