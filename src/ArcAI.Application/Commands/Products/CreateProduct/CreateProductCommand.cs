using ArcAI.Application.Common.Interfaces;
using ArcAI.Application.DTOs;

namespace ArcAI.Application.Commands.Products.CreateProduct;

/// <summary>
/// Command for creating a new product.
/// </summary>
public sealed record CreateProductCommand : ICommand<ProductDto>
{
    /// <summary>
    /// Gets the product SKU.
    /// </summary>
    public string Sku { get; init; } = default!;

    /// <summary>
    /// Gets the product name.
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Gets the product description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the price amount.
    /// </summary>
    public decimal Price { get; init; }

    /// <summary>
    /// Gets the price currency (ISO 4217 code).
    /// </summary>
    public string Currency { get; init; } = "USD";

    /// <summary>
    /// Gets the category identifier.
    /// </summary>
    public Guid? CategoryId { get; init; }
}