using ArcAI.Application.Common.Interfaces;
using ArcAI.Application.Common.Models;
using ArcAI.Application.DTOs;
using ArcAI.Domain.Enums;

namespace ArcAI.Application.Queries.Products.SearchProducts;

/// <summary>
/// Query for searching products with pagination.
/// </summary>
public sealed record SearchProductsQuery : IQuery<PaginatedList<ProductDto>>
{
    /// <summary>
    /// Gets the search term (searches name and SKU).
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Gets the status filter.
    /// </summary>
    public ProductStatus? Status { get; init; }

    /// <summary>
    /// Gets the category filter.
    /// </summary>
    public Guid? CategoryId { get; init; }

    /// <summary>
    /// Gets the minimum price filter.
    /// </summary>
    public decimal? MinPrice { get; init; }

    /// <summary>
    /// Gets the maximum price filter.
    /// </summary>
    public decimal? MaxPrice { get; init; }

    /// <summary>
    /// Gets the page number (1-based).
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int PageSize { get; init; } = 20;
}