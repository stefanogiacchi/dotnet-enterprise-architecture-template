using ArcAI.Domain.Common;

namespace ArcAI.Domain.Interfaces;

/// <summary>
/// Generic repository interface for aggregate roots.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
/// <typeparam name="TId">The identifier type.</typeparam>
public interface IRepository<T, TId>
    where T : AggregateRoot<TId>
    where TId : notnull
{
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity if found, null otherwise.</returns>
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of all entities.</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities matching a specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of matching entities.</returns>
    Task<IReadOnlyList<T>> GetAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity matching a specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The first matching entity, or null if none found.</returns>
    Task<T?> GetFirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching a specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The count of matching entities.</returns>
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity matches a specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any entity matches, false otherwise.</returns>
    Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added entity.</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}

/// <summary>
/// Simplified repository interface using Guid as identifier.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
public interface IRepository<T> : IRepository<T, Guid> where T : AggregateRoot<Guid>
{
}