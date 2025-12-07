using ArcAI.Domain.Entities;
using ArcAI.Domain.ValueObjects;
using FluentValidation;

namespace ArcAI.Application.Commands.Products.CreateProduct;

/// <summary>
/// Validator for CreateProductCommand.
/// </summary>
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("SKU is required.")
            .MinimumLength(Sku.MinLength)
            .WithMessage($"SKU must be at least {Sku.MinLength} characters.")
            .MaximumLength(Sku.MaxLength)
            .WithMessage($"SKU cannot exceed {Sku.MaxLength} characters.")
            .Matches(@"^[A-Za-z0-9\-]+$")
            .WithMessage("SKU can only contain letters, numbers, and hyphens.");

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
            .WithMessage("Price cannot be negative.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required.")
            .Length(3)
            .WithMessage("Currency must be a 3-letter ISO code.")
            .Matches("^[A-Za-z]{3}$")
            .WithMessage("Currency must be a valid ISO currency code.");
    }
}