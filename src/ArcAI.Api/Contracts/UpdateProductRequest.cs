namespace ArcAI.Api.Contracts;

/// <summary>
/// Request model for updating an existing product.
/// </summary>
public sealed class UpdateProductRequest
{
    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Gets or sets the price currency (ISO 4217 code).
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    public Guid? CategoryId { get; set; }
}
