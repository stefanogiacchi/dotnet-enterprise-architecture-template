using ArcAI.Application.Common.Interfaces;
using ArcAI.Application.DTOs;

namespace ArcAI.Application.Queries.Products.GetProductById;

/// <summary>
/// Query for retrieving a product by its identifier.
/// </summary>
public sealed record GetProductByIdQuery : IQuery<ProductDto?>
{
    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public Guid Id { get; init; }
}