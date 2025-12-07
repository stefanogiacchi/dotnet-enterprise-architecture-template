namespace ArcAI.Domain.Exceptions;

/// <summary>
/// Exception thrown when an operation would result in a conflict,
/// such as creating a duplicate resource.
/// </summary>
public class ConflictException : DomainException
{
    /// <summary>
    /// Gets the name of the entity type that has a conflict.
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    /// Gets the conflicting value.
    /// </summary>
    public object ConflictingValue { get; }

    /// <summary>
    /// Initializes a new instance of the ConflictException class.
    /// </summary>
    /// <param name="entityName">The name of the entity type.</param>
    /// <param name="conflictingValue">The conflicting value.</param>
    public ConflictException(string entityName, object conflictingValue)
        : base($"A conflict occurred for entity '{entityName}' with value '{conflictingValue}'.", "CONFLICT")
    {
        EntityName = entityName;
        ConflictingValue = conflictingValue;
    }

    /// <summary>
    /// Initializes a new instance of the ConflictException class with a custom message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="entityName">The name of the entity type.</param>
    /// <param name="conflictingValue">The conflicting value.</param>
    public ConflictException(string message, string entityName, object conflictingValue)
        : base(message, "CONFLICT")
    {
        EntityName = entityName;
        ConflictingValue = conflictingValue;
    }

    /// <summary>
    /// Creates a ConflictException for a duplicate entity.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="conflictingValue">The conflicting value.</param>
    /// <returns>A new ConflictException instance.</returns>
    public static ConflictException DuplicateFor<TEntity>(object conflictingValue)
    {
        return new ConflictException(
            $"A '{typeof(TEntity).Name}' with the same unique value '{conflictingValue}' already exists.",
            typeof(TEntity).Name,
            conflictingValue);
    }
}