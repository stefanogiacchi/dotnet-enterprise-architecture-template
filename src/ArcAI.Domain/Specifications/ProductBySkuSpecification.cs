using ArcAI.Domain.Common;
using ArcAI.Domain.Entities;
using ArcAI.Domain.Enums;

namespace ArcAI.Domain.Specifications;

/// <summary>
/// Specification for finding a product by SKU.
/// </summary>
public sealed class ProductBySkuSpecification : Specification<Product>
{
    public ProductBySkuSpecification(string sku)
        : base(p => p.Sku.Value == sku.ToUpperInvariant())
    {
    }
}

/// <summary>
/// Specification for finding products by status.
/// </summary>
public sealed class ProductByStatusSpecification : Specification<Product>
{
    public ProductByStatusSpecification(ProductStatus status)
        : base(p => p.Status == status)
    {
    }
}

/// <summary>
/// Specification for finding active (published) products.
/// </summary>
public sealed class ActiveProductsSpecification : Specification<Product>
{
    public ActiveProductsSpecification()
        : base(p => p.Status == ProductStatus.Published)
    {
    }
}

/// <summary>
/// Specification for finding products by category.
/// </summary>
public sealed class ProductsByCategorySpecification : Specification<Product>
{
    public ProductsByCategorySpecification(Guid categoryId)
        : base(p => p.CategoryId == categoryId)
    {
    }
}

/// <summary>
/// Specification for finding products with price in range.
/// </summary>
public sealed class ProductsByPriceRangeSpecification : Specification<Product>
{
    public ProductsByPriceRangeSpecification(decimal minPrice, decimal maxPrice)
        : base(p => p.Price.Amount >= minPrice && p.Price.Amount <= maxPrice)
    {
    }
}

/// <summary>
/// Specification for searching products by name (contains).
/// </summary>
public sealed class ProductsByNameSearchSpecification : Specification<Product>
{
    public ProductsByNameSearchSpecification(string searchTerm)
        : base(p => p.Name.ToLower().Contains(searchTerm.ToLower()))
    {
    }
}

/// <summary>
/// Composite specification for advanced product search.
/// </summary>
public sealed class ProductSearchSpecification : Specification<Product>
{
    public ProductSearchSpecification(
        string? searchTerm = null,
        ProductStatus? status = null,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int skip = 0,
        int take = 20,
        bool orderByPriceDescending = false)
    {
        // Build criteria expression
        AddCriteria(p =>
            (string.IsNullOrEmpty(searchTerm) ||
             p.Name.ToLower().Contains(searchTerm.ToLower()) ||
             p.Sku.Value.ToLower().Contains(searchTerm.ToLower())) &&
            (!status.HasValue || p.Status == status.Value) &&
            (!categoryId.HasValue || p.CategoryId == categoryId.Value) &&
            (!minPrice.HasValue || p.Price.Amount >= minPrice.Value) &&
            (!maxPrice.HasValue || p.Price.Amount <= maxPrice.Value));

        // Apply ordering
        if (orderByPriceDescending)
        {
            ApplyOrderByDescending(p => p.Price.Amount);
        }
        else
        {
            ApplyOrderBy(p => p.Name);
        }

        // Apply paging
        ApplyPaging(skip, take);
    }
}