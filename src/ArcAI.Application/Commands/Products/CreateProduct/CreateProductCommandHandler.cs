using ArcAI.Application.Common.Interfaces;
using ArcAI.Application.DTOs;
using ArcAI.Application.Extensions;  
using ArcAI.Domain.Entities;
using ArcAI.Domain.Exceptions;
using ArcAI.Domain.Interfaces;
using ArcAI.Domain.ValueObjects;

namespace ArcAI.Application.Commands.Products.CreateProduct;

/// <summary>
/// Handler for CreateProductCommand.
/// </summary>
public sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Check if SKU already exists
        if (await _productRepository.SkuExistsAsync(request.Sku, cancellationToken: cancellationToken))
        {
            throw ConflictException.DuplicateFor<Product>(request.Sku);
        }

        // Create value objects
        var sku = new Sku(request.Sku);
        var price = new Money(request.Price, request.Currency);

        // Create the product using domain factory method
        var product = Product.Create(
            sku,
            request.Name,
            request.Description,
            price,
            request.CategoryId,
            _currentUserService.UserId);

        // Persist
        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return DTO (ToDto is now available via using ArcAI.Application.Extensions)
        return product.ToDto();
    }
}