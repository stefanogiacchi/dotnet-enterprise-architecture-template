namespace ArcAI.Domain.Common;

/// <summary>
/// Interface for entities that support audit tracking.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who created the entity.
    /// </summary>
    string? CreatedBy { get; }

    /// <summary>
    /// Gets the date and time when the entity was last modified.
    /// </summary>
    DateTime? UpdatedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who last modified the entity.
    /// </summary>
    string? UpdatedBy { get; }
}