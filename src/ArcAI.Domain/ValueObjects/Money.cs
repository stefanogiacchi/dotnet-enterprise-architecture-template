using ArcAI.Domain.Common;
using ArcAI.Domain.Exceptions;

namespace ArcAI.Domain.ValueObjects;

/// <summary>
/// Represents a monetary value with currency.
/// Implements the Value Object pattern - immutable and compared by value.
/// </summary>
public sealed class Money : ValueObject
{
    /// <summary>
    /// Gets the monetary amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the currency code (ISO 4217).
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Initializes a new Money value object.
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="currency">The currency code.</param>
    /// <exception cref="DomainException">Thrown when currency is invalid.</exception>
    public Money(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency code is required.", "INVALID_CURRENCY");

        if (currency.Length != 3)
            throw new DomainException("Currency code must be a 3-letter ISO code.", "INVALID_CURRENCY");

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    /// <summary>
    /// Creates a Money object with zero amount in the specified currency.
    /// </summary>
    /// <param name="currency">The currency code.</param>
    /// <returns>A Money object with zero amount.</returns>
    public static Money Zero(string currency) => new(0, currency);

    /// <summary>
    /// Creates a Money object in USD.
    /// </summary>
    /// <param name="amount">The amount in USD.</param>
    /// <returns>A Money object in USD.</returns>
    public static Money InUsd(decimal amount) => new(amount, "USD");

    /// <summary>
    /// Creates a Money object in EUR.
    /// </summary>
    /// <param name="amount">The amount in EUR.</param>
    /// <returns>A Money object in EUR.</returns>
    public static Money InEur(decimal amount) => new(amount, "EUR");

    /// <summary>
    /// Adds two Money values together.
    /// </summary>
    /// <exception cref="DomainException">Thrown when currencies don't match.</exception>
    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// Subtracts a Money value from this one.
    /// </summary>
    /// <exception cref="DomainException">Thrown when currencies don't match.</exception>
    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    /// <summary>
    /// Multiplies the money by a factor.
    /// </summary>
    public Money Multiply(decimal factor) => new(Amount * factor, Currency);

    /// <summary>
    /// Checks if this money value is positive (greater than zero).
    /// </summary>
    public bool IsPositive() => Amount > 0;

    /// <summary>
    /// Checks if this money value is negative (less than zero).
    /// </summary>
    public bool IsNegative() => Amount < 0;

    /// <summary>
    /// Checks if this money value is zero.
    /// </summary>
    public bool IsZero() => Amount == 0;

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException(
                $"Cannot perform operation between different currencies: {Currency} and {other.Currency}.",
                "CURRENCY_MISMATCH");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money money, decimal factor) => money.Multiply(factor);
    public static Money operator *(decimal factor, Money money) => money.Multiply(factor);

    /// <summary>
    /// Parameterless constructor for ORM support.
    /// </summary>
    private Money()
    {
        Amount = 0;
        Currency = "USD";
    }
}