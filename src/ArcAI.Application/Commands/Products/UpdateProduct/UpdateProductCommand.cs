using ArcAI.Application.Common.Interfaces;
using ArcAI.Application.DTOs;

namespace ArcAI.Application.Commands.Products.UpdateProduct;

/// <summary>
/// Command for updating an existing product.
/// </summary>
public sealed record UpdateProductCommand : ICommand<ProductDto>
{
    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public Guid Id { get; init; }

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
    public decimal? Price { get; init; }

    /// <summary>
    /// Gets the price currency (ISO 4217 code).
    /// </summary>
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the category identifier.
    /// </summary>
    public Guid? CategoryId { get; init; }
}