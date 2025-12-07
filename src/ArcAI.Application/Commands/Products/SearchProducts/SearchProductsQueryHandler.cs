using ArcAI.Application.Common.Interfaces;
using ArcAI.Application.Common.Models;
using ArcAI.Application.DTOs;
using ArcAI.Domain.Interfaces;

namespace ArcAI.Application.Queries.Products.SearchProducts;

/// <summary>
/// Handler for SearchProductsQuery.
/// </summary>
public sealed class SearchProductsQueryHandler : IQueryHandler<SearchProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public SearchProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PaginatedList<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.PageNumber - 1) * request.PageSize;

        var (items, totalCount) = await _productRepository.SearchAsync(
            searchTerm: request.SearchTerm,
            status: request.Status,
            categoryId: request.CategoryId,
            minPrice: request.MinPrice,
            maxPrice: request.MaxPrice,
            skip: skip,
            take: request.PageSize,
            cancellationToken: cancellationToken);

        var dtos = items.ToDto();

        return new PaginatedList<ProductDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize);
    }
}