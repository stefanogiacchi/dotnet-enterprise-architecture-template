using ArcAI.Domain.Common;
using ArcAI.Domain.Enums;
using ArcAI.Domain.Events;
using ArcAI.Domain.Exceptions;
using ArcAI.Domain.ValueObjects;

namespace ArcAI.Domain.Entities;

/// <summary>
/// Represents a product in the catalog domain.
/// Implements the aggregate root pattern for the Product aggregate.
/// </summary>
public sealed class Product : AggregateRoot<Guid>, IAuditableEntity
{
    /// <summary>
    /// Gets the product SKU.
    /// </summary>
    public Sku Sku { get; private set; }

    /// <summary>
    /// Gets the product name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the product description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the product price.
    /// </summary>
    public Money Price { get; private set; }

    /// <summary>
    /// Gets the product status.
    /// </summary>
    public ProductStatus Status { get; private set; }

    /// <summary>
    /// Gets the category identifier.
    /// </summary>
    public Guid? CategoryId { get; private set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// Maximum length for product name.
    /// </summary>
    public const int MaxNameLength = 200;

    /// <summary>
    /// Maximum length for product description.
    /// </summary>
    public const int MaxDescriptionLength = 2000;

    /// <summary>
    /// Private constructor for controlled creation.
    /// </summary>
    private Product(
        Guid id,
        Sku sku,
        string name,
        string? description,
        Money price,
        Guid? categoryId,
        string? createdBy)
        : base(id)
    {
        Sku = sku;
        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        Status = ProductStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Creates a new product with validation.
    /// </summary>
    /// <param name="sku">The product SKU.</param>
    /// <param name="name">The product name.</param>
    /// <param name="description">The product description.</param>
    /// <param name="price">The product price.</param>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="createdBy">The creator identifier.</param>
    /// <returns>A new Product instance.</returns>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public static Product Create(
        Sku sku,
        string name,
        string? description,
        Money price,
        Guid? categoryId = null,
        string? createdBy = null)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(price);

        var product = new Product(
            Guid.NewGuid(),
            sku,
            name.Trim(),
            description?.Trim(),
            price,
            categoryId,
            createdBy);

        product.AddDomainEvent(new ProductCreatedEvent(
            product.Id,
            product.Sku.Value,
            product.Name,
            product.Price.Amount,
            product.Price.Currency));

        return product;
    }

    /// <summary>
    /// Updates the product details.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="description">The new description.</param>
    /// <param name="updatedBy">The updater identifier.</param>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public void UpdateDetails(string name, string? description, string? updatedBy = null)
    {
        ValidateName(name);
        ValidateDescription(description);

        Name = name.Trim();
        Description = description?.Trim();
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the product price.
    /// </summary>
    /// <param name="newPrice">The new price.</param>
    /// <param name="updatedBy">The updater identifier.</param>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public void UpdatePrice(Money newPrice, string? updatedBy = null)
    {
        ValidatePrice(newPrice);

        var oldPrice = Price;
        Price = newPrice;
        SetUpdated(updatedBy);

        AddDomainEvent(new ProductPriceChangedEvent(
            Id,
            oldPrice.Amount,
            oldPrice.Currency,
            newPrice.Amount,
            newPrice.Currency));
    }

    /// <summary>
    /// Changes the product category.
    /// </summary>
    /// <param name="categoryId">The new category identifier.</param>
    /// <param name="updatedBy">The updater identifier.</param>
    public void ChangeCategory(Guid? categoryId, string? updatedBy = null)
    {
        CategoryId = categoryId;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Publishes the product, making it available.
    /// </summary>
    /// <param name="updatedBy">The updater identifier.</param>
    /// <exception cref="DomainException">Thrown when the product cannot be published.</exception>
    public void Publish(string? updatedBy = null)
    {
        if (Status == ProductStatus.Published)
            throw new DomainException("Product is already published.", "ALREADY_PUBLISHED");

        if (Status == ProductStatus.Discontinued)
            throw new DomainException("Cannot publish a discontinued product.", "CANNOT_PUBLISH_DISCONTINUED");

        Status = ProductStatus.Published;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Discontinues the product.
    /// </summary>
    /// <param name="updatedBy">The updater identifier.</param>
    /// <exception cref="DomainException">Thrown when the product is already discontinued.</exception>
    public void Discontinue(string? updatedBy = null)
    {
        if (Status == ProductStatus.Discontinued)
            throw new DomainException("Product is already discontinued.", "ALREADY_DISCONTINUED");

        Status = ProductStatus.Discontinued;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Reactivates a discontinued product back to draft status.
    /// </summary>
    /// <param name="updatedBy">The updater identifier.</param>
    /// <exception cref="DomainException">Thrown when the product is not discontinued.</exception>
    public void Reactivate(string? updatedBy = null)
    {
        if (Status != ProductStatus.Discontinued)
            throw new DomainException("Only discontinued products can be reactivated.", "NOT_DISCONTINUED");

        Status = ProductStatus.Draft;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Checks if the product is available for sale.
    /// </summary>
    public bool IsAvailable => Status == ProductStatus.Published;

    private void SetUpdated(string? updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        IncrementVersion();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty.", "INVALID_NAME");

        if (name.Length > MaxNameLength)
            throw new DomainException($"Product name cannot exceed {MaxNameLength} characters.", "NAME_TOO_LONG");
    }

    private static void ValidateDescription(string? description)
    {
        if (description is not null && description.Length > MaxDescriptionLength)
            throw new DomainException($"Product description cannot exceed {MaxDescriptionLength} characters.", "DESCRIPTION_TOO_LONG");
    }

    private static void ValidatePrice(Money price)
    {
        if (price.Amount < 0)
            throw new DomainException("Product price cannot be negative.", "INVALID_PRICE");
    }

    /// <summary>
    /// Parameterless constructor for ORM support.
    /// </summary>
    private Product() : base()
    {
        Sku = null!;
        Name = null!;
        Price = null!;
    }
}