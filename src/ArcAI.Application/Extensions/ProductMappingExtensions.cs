using ArcAI.Application.DTOs;
using ArcAI.Domain.Entities;

namespace ArcAI.Application.Extensions;

/// <summary>
/// Extension methods for mapping between Product entity and ProductDto.
/// </summary>
public static class ProductMappingExtensions
{
    /// <summary>
    /// Maps a Product entity to a ProductDto.
    /// </summary>
    /// <param name="product">The product entity to map.</param>
    /// <returns>A ProductDto instance.</returns>
    public static ProductDto ToDto(this Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        return new ProductDto
        {
            Id = product.Id,
            Sku = product.Sku.Value,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            Status = product.Status.ToString(),
            CategoryId = product.CategoryId,
            CreatedAt = product.CreatedAt,
            CreatedBy = product.CreatedBy,
            UpdatedAt = product.UpdatedAt,
            UpdatedBy = product.UpdatedBy,
            Version = product.Version
        };
    }

    /// <summary>
    /// Maps a collection of Product entities to ProductDto collection.
    /// </summary>
    /// <param name="products">The product entities to map.</param>
    /// <returns>A collection of ProductDto instances.</returns>
    public static IEnumerable<ProductDto> ToDto(this IEnumerable<Product> products)
    {
        if (products == null)
            throw new ArgumentNullException(nameof(products));

        return products.Select(p => p.ToDto());
    }

    /// <summary>
    /// Maps a collection of Product entities to a list of ProductDto.
    /// </summary>
    /// <param name="products">The product entities to map.</param>
    /// <returns>A list of ProductDto instances.</returns>
    public static List<ProductDto> ToDtoList(this IEnumerable<Product> products)
    {
        if (products == null)
            throw new ArgumentNullException(nameof(products));

        return products.Select(p => p.ToDto()).ToList();
    }
}