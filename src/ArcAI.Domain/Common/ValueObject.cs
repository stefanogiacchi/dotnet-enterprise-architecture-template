namespace ArcAI.Domain.Common;

/// <summary>
/// Base class for Value Objects implementing DDD Value Object pattern.
/// Value objects are immutable and compared by their attribute values.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Gets the components used for equality comparison.
    /// Override this method to specify which properties define equality.
    /// </summary>
    /// <returns>An enumerable of equality components.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// Determines whether this value object equals another value object.
    /// </summary>
    public bool Equals(ValueObject? other)
    {
        if (other is null) return false;
        if (GetType() != other.GetType()) return false;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Determines whether the specified object equals this value object.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is ValueObject valueObject && Equals(valueObject);
    }

    /// <summary>
    /// Returns the hash code for this value object based on its components.
    /// </summary>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(17, (current, component) =>
                current * 31 + (component?.GetHashCode() ?? 0));
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}