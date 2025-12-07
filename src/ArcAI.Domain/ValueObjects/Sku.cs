using ArcAI.Domain.Common;
using ArcAI.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace ArcAI.Domain.ValueObjects;

/// <summary>
/// Represents a Stock Keeping Unit (SKU) identifier.
/// Implements the Value Object pattern.
/// </summary>
public sealed partial class Sku : ValueObject
{
    /// <summary>
    /// Gets the SKU value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Maximum length allowed for a SKU.
    /// </summary>
    public const int MaxLength = 50;

    /// <summary>
    /// Minimum length required for a SKU.
    /// </summary>
    public const int MinLength = 3;

    /// <summary>
    /// Initializes a new SKU value object.
    /// </summary>
    /// <param name="value">The SKU value.</param>
    /// <exception cref="DomainException">Thrown when SKU is invalid.</exception>
    public Sku(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("SKU cannot be empty.", "INVALID_SKU");

        value = value.Trim().ToUpperInvariant();

        if (value.Length < MinLength)
            throw new DomainException($"SKU must be at least {MinLength} characters.", "INVALID_SKU");

        if (value.Length > MaxLength)
            throw new DomainException($"SKU cannot exceed {MaxLength} characters.", "INVALID_SKU");

        if (!SkuRegex().IsMatch(value))
            throw new DomainException("SKU can only contain letters, numbers, and hyphens.", "INVALID_SKU");

        Value = value;
    }

    /// <summary>
    /// Creates a SKU from a string value.
    /// </summary>
    /// <param name="value">The SKU string.</param>
    /// <returns>A new SKU instance.</returns>
    public static Sku From(string value) => new(value);

    /// <summary>
    /// Attempts to create a SKU from a string value.
    /// </summary>
    /// <param name="value">The SKU string.</param>
    /// <param name="sku">The resulting SKU if valid.</param>
    /// <returns>True if the SKU is valid, false otherwise.</returns>
    public static bool TryCreate(string value, out Sku? sku)
    {
        try
        {
            sku = new Sku(value);
            return true;
        }
        catch
        {
            sku = null;
            return false;
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Sku sku) => sku.Value;

    [GeneratedRegex(@"^[A-Z0-9\-]+$")]
    private static partial Regex SkuRegex();

    /// <summary>
    /// Parameterless constructor for ORM support.
    /// </summary>
    private Sku()
    {
        Value = string.Empty;
    }
}