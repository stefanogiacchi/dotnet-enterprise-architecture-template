namespace ArcAI.Api.Contracts;

/// <summary>
/// Request model for searching products with pagination and filtering.
/// </summary>
public sealed class SearchProductsRequest
{
    /// <summary>
    /// Gets or sets the search term (searches name and SKU).
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets or sets the status filter (Draft, Published, Discontinued).
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the category filter.
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the minimum price filter.
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Gets or sets the maximum price filter.
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Gets or sets the page number (1-based, default: 1).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size (default: 20, max: 100).
    /// </summary>
    public int PageSize { get; set; } = 20;
}
