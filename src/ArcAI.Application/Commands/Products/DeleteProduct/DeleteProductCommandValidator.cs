using FluentValidation;

namespace ArcAI.Application.Commands.Products.DeleteProduct;

/// <summary>
/// Validator for DeleteProductCommand.
/// </summary>
public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Product ID is required.");
    }
}