using ArcAI.Domain.Entities;
using ArcAI.Domain.Enums;

namespace ArcAI.Domain.Interfaces;

/// <summary>
/// Repository interface for Product aggregate.
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// Gets a product by its SKU.
    /// </summary>
    /// <param name="sku">The product SKU.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found, null otherwise.</returns>
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a SKU already exists.
    /// </summary>
    /// <param name="sku">The SKU to check.</param>
    /// <param name="excludeProductId">Optional product ID to exclude from check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if SKU exists, false otherwise.</returns>
    Task<bool> SkuExistsAsync(string sku, Guid? excludeProductId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products by category.
    /// </summary>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of products in the category.</returns>
    Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products by status.
    /// </summary>
    /// <param name="status">The product status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of products with the status.</returns>
    Task<IReadOnlyList<Product>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches products with pagination.
    /// </summary>
    /// <param name="searchTerm">Optional search term for name/SKU.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="categoryId">Optional category filter.</param>
    /// <param name="minPrice">Optional minimum price filter.</param>
    /// <param name="maxPrice">Optional maximum price filter.</param>
    /// <param name="skip">Number of items to skip.</param>
    /// <param name="take">Number of items to take.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple with the list of products and total count.</returns>
    Task<(IReadOnlyList<Product> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        ProductStatus? status = null,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);
}