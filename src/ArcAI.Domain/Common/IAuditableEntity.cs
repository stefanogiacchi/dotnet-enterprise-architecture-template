namespace ArcAI.Domain.Common;

/// <summary>
/// Interface for entities that track audit information.
/// Provides creation and modification timestamps with user tracking.
/// </summary>
public interface IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy
    {
        get; set;
    }
}