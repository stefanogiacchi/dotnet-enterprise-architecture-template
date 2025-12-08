using ArcAI.Application.Common.Interfaces;
using ArcAI.Application.DTOs;
using ArcAI.Application.Extensions;  // ✅ ADD THIS LINE
using ArcAI.Domain.Entities;
using ArcAI.Domain.Exceptions;
using ArcAI.Domain.Interfaces;
using ArcAI.Domain.ValueObjects;

namespace ArcAI.Application.Commands.Products.UpdateProduct;

/// <summary>
/// Handler for UpdateProductCommand.
/// </summary>
public sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // Get existing product
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            throw NotFoundException.For<Product>(request.Id);
        }

        // Update details
        product.UpdateDetails(request.Name, request.Description, _currentUserService.UserId);

        // Update price if provided
        if (request.Price.HasValue && request.Currency is not null)
        {
            var newPrice = new Money(request.Price.Value, request.Currency);
            product.UpdatePrice(newPrice, _currentUserService.UserId);
        }

        // Update category
        product.ChangeCategory(request.CategoryId, _currentUserService.UserId);

        // Persist
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return DTO
        return product.ToDto();
    }
}