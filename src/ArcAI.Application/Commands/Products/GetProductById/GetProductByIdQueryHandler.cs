using ArcAI.Application.Common.Interfaces;
using ArcAI.Application.DTOs;
using ArcAI.Domain.Interfaces;

namespace ArcAI.Application.Queries.Products.GetProductById;

/// <summary>
/// Handler for GetProductByIdQuery.
/// </summary>
public sealed class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        return product?.ToDto();
    }
}