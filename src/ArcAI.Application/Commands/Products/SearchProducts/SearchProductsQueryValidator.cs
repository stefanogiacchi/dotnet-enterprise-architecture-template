using FluentValidation;

namespace ArcAI.Application.Queries.Products.SearchProducts;

/// <summary>
/// Validator for SearchProductsQuery.
/// </summary>
public sealed class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum price cannot be negative.")
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Maximum price cannot be negative.")
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x)
            .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
            .WithMessage("Minimum price cannot be greater than maximum price.");
    }
}