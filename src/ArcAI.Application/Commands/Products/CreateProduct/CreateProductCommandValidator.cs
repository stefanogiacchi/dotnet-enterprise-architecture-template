using ArcAI.Application.Commands.Products.CreateProduct;
using ArcAI.Domain.Interfaces;
using FluentValidation;

namespace ArcAI.Application.Commands.Products.CreateProduct;

/// <summary>
/// Validator for CreateProductCommand.
/// Demonstrates comprehensive FluentValidation patterns.
/// </summary>
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        // SKU validation
        RuleFor(x => x.Sku)
            .NotEmpty()
                .WithMessage("SKU is required")
                .WithErrorCode("PRODUCT_SKU_REQUIRED")
            .MaximumLength(50)
                .WithMessage("SKU cannot exceed 50 characters")
                .WithErrorCode("PRODUCT_SKU_TOO_LONG")
            .Matches(@"^[A-Z0-9\-]+$")
                .WithMessage("SKU must contain only uppercase letters, numbers, and hyphens")
                .WithErrorCode("PRODUCT_SKU_INVALID_FORMAT")
            .MustAsync(BeUniqueSku)
                .WithMessage("A product with this SKU already exists")
                .WithErrorCode("PRODUCT_SKU_DUPLICATE");

        // Name validation
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Product name is required")
                .WithErrorCode("PRODUCT_NAME_REQUIRED")
            .MaximumLength(200)
                .WithMessage("Product name cannot exceed 200 characters")
                .WithErrorCode("PRODUCT_NAME_TOO_LONG")
            .MinimumLength(3)
                .WithMessage("Product name must be at least 3 characters")
                .WithErrorCode("PRODUCT_NAME_TOO_SHORT");

        // Description validation (optional)
        RuleFor(x => x.Description)
            .MaximumLength(2000)
                .WithMessage("Description cannot exceed 2000 characters")
                .WithErrorCode("PRODUCT_DESCRIPTION_TOO_LONG")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        // Price validation
        RuleFor(x => x.Price)
            .GreaterThan(0)
                .WithMessage("Price must be greater than zero")
                .WithErrorCode("PRODUCT_PRICE_INVALID")
            .PrecisionScale(18, 2, false) // ✅ FIXED: precision=18, scale=2
                .WithMessage("Price can have maximum 2 decimal places and 18 total digits")
                .WithErrorCode("PRODUCT_PRICE_PRECISION");

        // Currency validation
        RuleFor(x => x.Currency)
            .NotEmpty()
                .WithMessage("Currency is required")
                .WithErrorCode("PRODUCT_CURRENCY_REQUIRED")
            .Length(3)
                .WithMessage("Currency must be a 3-letter ISO code")
                .WithErrorCode("PRODUCT_CURRENCY_INVALID_LENGTH")
            .Matches(@"^[A-Z]{3}$")
                .WithMessage("Currency must be uppercase 3-letter code (e.g., USD, EUR)")
                .WithErrorCode("PRODUCT_CURRENCY_INVALID_FORMAT");

        // CategoryId validation (optional)
        RuleFor(x => x.CategoryId)
            .Must(id => id != Guid.Empty)
                .WithMessage("Invalid category ID")
                .WithErrorCode("PRODUCT_CATEGORY_INVALID")
            .When(x => x.CategoryId.HasValue);
    }

    /// <summary>
    /// Validates that the SKU is unique.
    /// </summary>
    private async Task<bool> BeUniqueSku(string sku, CancellationToken cancellationToken)
    {
        return !await _productRepository.SkuExistsAsync(sku, cancellationToken: cancellationToken);
    }
}