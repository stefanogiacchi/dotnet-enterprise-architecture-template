using ArcAI.Domain.Entities;
using FluentValidation;

namespace ArcAI.Application.Commands.Products.UpdateProduct;

/// <summary>
/// Validator for UpdateProductCommand.
/// </summary>
public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(Product.MaxNameLength)
            .WithMessage($"Name cannot exceed {Product.MaxNameLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(Product.MaxDescriptionLength)
            .WithMessage($"Description cannot exceed {Product.MaxDescriptionLength} characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price cannot be negative.")
            .When(x => x.Price.HasValue);

        RuleFor(x => x.Currency)
            .Length(3)
            .WithMessage("Currency must be a 3-letter ISO code.")
            .Matches("^[A-Za-z]{3}$")
            .WithMessage("Currency must be a valid ISO currency code.")
            .When(x => x.Currency is not null);

        // If price is provided, currency must also be provided
        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required when updating price.")
            .When(x => x.Price.HasValue);
    }
}