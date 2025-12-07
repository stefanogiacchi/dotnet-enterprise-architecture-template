namespace ArcAI.Domain.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found.
/// </summary>
public class NotFoundException : DomainException
{
    /// <summary>
    /// Gets the name of the entity type that was not found.
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    /// Gets the key/identifier that was searched for.
    /// </summary>
    public object Key { get; }

    /// <summary>
    /// Initializes a new instance of the NotFoundException class.
    /// </summary>
    /// <param name="entityName">The name of the entity type.</param>
    /// <param name="key">The key that was searched for.</param>
    public NotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' with key '{key}' was not found.", "NOT_FOUND")
    {
        EntityName = entityName;
        Key = key;
    }

    /// <summary>
    /// Creates a NotFoundException for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="key">The key that was searched for.</param>
    /// <returns>A new NotFoundException instance.</returns>
    public static NotFoundException For<TEntity>(object key)
    {
        return new NotFoundException(typeof(TEntity).Name, key);
    }
}