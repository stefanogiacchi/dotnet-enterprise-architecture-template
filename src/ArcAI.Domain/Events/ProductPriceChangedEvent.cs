using ArcAI.Domain.Common;

namespace ArcAI.Domain.Events;

/// <summary>
/// Domain event raised when a product's price changes.
/// </summary>
public sealed record ProductPriceChangedEvent : DomainEventBase
{
    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public Guid ProductId { get; }

    /// <summary>
    /// Gets the old price amount.
    /// </summary>
    public decimal OldPriceAmount { get; }

    /// <summary>
    /// Gets the old price currency.
    /// </summary>
    public string OldPriceCurrency { get; }

    /// <summary>
    /// Gets the new price amount.
    /// </summary>
    public decimal NewPriceAmount { get; }

    /// <summary>
    /// Gets the new price currency.
    /// </summary>
    public string NewPriceCurrency { get; }

    /// <summary>
    /// Initializes a new ProductPriceChangedEvent.
    /// </summary>
    public ProductPriceChangedEvent(
        Guid productId,
        decimal oldPriceAmount,
        string oldPriceCurrency,
        decimal newPriceAmount,
        string newPriceCurrency)
    {
        ProductId = productId;
        OldPriceAmount = oldPriceAmount;
        OldPriceCurrency = oldPriceCurrency;
        NewPriceAmount = newPriceAmount;
        NewPriceCurrency = newPriceCurrency;
    }
}