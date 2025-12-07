using ArcAI.Application.Common.Interfaces;
using ArcAI.Domain.Entities;
using ArcAI.Domain.Exceptions;
using ArcAI.Domain.Interfaces;
using MediatR;

namespace ArcAI.Application.Commands.Products.DeleteProduct;

/// <summary>
/// Handler for DeleteProductCommand.
/// </summary>
public sealed class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // Get existing product
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            throw NotFoundException.For<Product>(request.Id);
        }

        // Delete
        await _productRepository.DeleteAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}