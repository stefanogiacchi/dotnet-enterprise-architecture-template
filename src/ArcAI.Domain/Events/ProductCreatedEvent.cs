using ArcAI.Domain.Common;

namespace ArcAI.Domain.Events;

/// <summary>
/// Domain event raised when a new product is created.
/// </summary>
public sealed record ProductCreatedEvent : DomainEventBase
{
    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public Guid ProductId { get; }

    /// <summary>
    /// Gets the product SKU.
    /// </summary>
    public string Sku { get; }

    /// <summary>
    /// Gets the product name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the product price amount.
    /// </summary>
    public decimal PriceAmount { get; }

    /// <summary>
    /// Gets the product price currency.
    /// </summary>
    public string PriceCurrency { get; }

    /// <summary>
    /// Initializes a new ProductCreatedEvent.
    /// </summary>
    public ProductCreatedEvent(
        Guid productId,
        string sku,
        string name,
        decimal priceAmount,
        string priceCurrency)
    {
        ProductId = productId;
        Sku = sku;
        Name = name;
        PriceAmount = priceAmount;
        PriceCurrency = priceCurrency;
    }
}