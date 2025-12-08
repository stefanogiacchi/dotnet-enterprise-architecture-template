using ArcAI.Domain.Common;
using ArcAI.Domain.Entities;
using ArcAI.Domain.Enums;
using ArcAI.Domain.Interfaces;
using ArcAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArcAI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Product entity.
/// Uses Entity Framework Core for data access with support for specifications.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductRepository> _logger;
    private readonly DbSet<Product> _products;

    public ProductRepository(
        AppDbContext context,
        ILogger<ProductRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _products = _context.Products;
    }

    #region IRepository<Product> Implementation

    /// <inheritdoc/>
    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting product by ID: {ProductId}", id);

        return await _products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all products");

        return await _products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Product>> GetAsync(
        ISpecification<Product> specification,
        CancellationToken cancellationToken = default)
    {
        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        _logger.LogDebug("Getting products with specification: {SpecificationType}",
            specification.GetType().Name);

        var query = _products.AsNoTracking();
        query = ApplySpecification(query, specification);

        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Product?> GetFirstOrDefaultAsync(
        ISpecification<Product> specification,
        CancellationToken cancellationToken = default)
    {
        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        _logger.LogDebug("Getting first product with specification: {SpecificationType}",
            specification.GetType().Name);

        var query = _products.AsNoTracking();
        query = ApplySpecification(query, specification);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Product> AddAsync(
        Product entity,
        CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _logger.LogDebug("Adding product: {ProductId}, SKU: {Sku}", entity.Id, entity.Sku.Value);

        await _products.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <inheritdoc/>
    public Task UpdateAsync(
        Product entity,
        CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _logger.LogDebug("Updating product: {ProductId}, SKU: {Sku}", entity.Id, entity.Sku.Value);

        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task DeleteAsync(
        Product entity,
        CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _logger.LogDebug("Deleting product: {ProductId}, SKU: {Sku}", entity.Id, entity.Sku.Value);

        _products.Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(
        ISpecification<Product>? specification = null,
        CancellationToken cancellationToken = default)
    {
        var query = _products.AsNoTracking();

        if (specification != null)
        {
            query = ApplySpecification(query, specification);
        }

        return await query.CountAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> AnyAsync(
        ISpecification<Product> specification,
        CancellationToken cancellationToken = default)
    {
        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        var query = _products.AsNoTracking();
        query = ApplySpecification(query, specification);

        return await query.AnyAsync(cancellationToken);
    }

    #endregion

    #region IProductRepository Specific Methods

    /// <inheritdoc/>
    public async Task<Product?> GetBySkuAsync(
        string sku,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be null or empty", nameof(sku));

        _logger.LogDebug("Getting product by SKU: {Sku}", sku);

        return await _products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Sku.Value == sku, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> SkuExistsAsync(
        string sku,
        Guid? excludeProductId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be null or empty", nameof(sku));

        _logger.LogDebug("Checking if SKU exists: {Sku}, ExcludeProductId: {ExcludeProductId}",
            sku, excludeProductId);

        var query = _products.AsNoTracking();

        if (excludeProductId.HasValue)
        {
            query = query.Where(p => p.Sku.Value == sku && p.Id != excludeProductId.Value);
        }
        else
        {
            query = query.Where(p => p.Sku.Value == sku);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting products by category: {CategoryId}", categoryId);

        return await _products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Product>> GetByStatusAsync(
        ProductStatus status,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting products by status: {Status}", status);

        return await _products
            .AsNoTracking()
            .Where(p => p.Status == status)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        ProductStatus? status = null,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        if (take < 1 || take > 100)
            throw new ArgumentException("Take must be between 1 and 100", nameof(take));

        if (skip < 0)
            throw new ArgumentException("Skip must be greater than or equal to 0", nameof(skip));

        _logger.LogDebug(
            "Searching products - SearchTerm: {SearchTerm}, Status: {Status}, CategoryId: {CategoryId}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}, Skip: {Skip}, Take: {Take}",
            searchTerm, status, categoryId, minPrice, maxPrice, skip, take);

        var query = _products.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.ToLower().Trim();
            query = query.Where(p =>
                p.Name.ToLower().Contains(normalizedSearchTerm) ||
                p.Sku.Value.ToLower().Contains(normalizedSearchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(normalizedSearchTerm)));
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price.Amount >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price.Amount <= maxPrice.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Applies a specification to a query.
    /// </summary>
    private IQueryable<Product> ApplySpecification(
        IQueryable<Product> query,
        ISpecification<Product> specification)
    {
        // Apply criteria
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Apply includes
        query = specification.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        // Apply string includes
        query = specification.IncludeStrings
            .Aggregate(query, (current, include) => current.Include(include));

        // Apply ordering
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Apply paging
        if (specification.IsPagingEnabled)
        {
            query = query
                .Skip(specification.Skip ?? 0)
                .Take(specification.Take ?? 20);
        }

        return query;
    }

    #endregion
}