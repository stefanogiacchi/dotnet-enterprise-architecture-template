namespace ArcAI.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a product.
/// </summary>
public enum ProductStatus
{
    /// <summary>
    /// Product is in draft status, not yet published.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Product is published and available.
    /// </summary>
    Published = 1,

    /// <summary>
    /// Product has been discontinued.
    /// </summary>
    Discontinued = 2
}