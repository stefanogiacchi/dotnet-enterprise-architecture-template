using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ArcAI.Shared.Utils;

/// <summary>
/// Guard clauses for argument validation.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Throws ArgumentNullException if the value is null.
    /// </summary>
    public static T NotNull<T>(
        [NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }

        return value;
    }

    /// <summary>
    /// Throws ArgumentException if the string is null or empty.
    /// </summary>
    public static string NotNullOrEmpty(
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be null or empty.", paramName);
        }

        return value;
    }

    /// <summary>
    /// Throws ArgumentException if the string is null, empty, or whitespace.
    /// </summary>
    public static string NotNullOrWhitespace(
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName);
        }

        return value;
    }

    /// <summary>
    /// Throws ArgumentOutOfRangeException if the value is not within the specified range.
    /// </summary>
    public static T InRange<T>(
        T value,
        T min,
        T max,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, $"Value must be between {min} and {max}.");
        }

        return value;
    }

    /// <summary>
    /// Throws ArgumentOutOfRangeException if the value is negative.
    /// </summary>
    public static T NotNegative<T>(
        T value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(default!) < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, "Value cannot be negative.");
        }

        return value;
    }

    /// <summary>
    /// Throws ArgumentOutOfRangeException if the value is not positive.
    /// </summary>
    public static T Positive<T>(
        T value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(default!) <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be positive.");
        }

        return value;
    }

    /// <summary>
    /// Throws ArgumentException if the Guid is empty.
    /// </summary>
    public static Guid NotEmpty(
        Guid value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Value cannot be an empty GUID.", paramName);
        }

        return value;
    }

    /// <summary>
    /// Throws ArgumentException if the collection is null or empty.
    /// </summary>
    public static IEnumerable<T> NotNullOrEmpty<T>(
        [NotNull] IEnumerable<T>? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value is null || !value.Any())
        {
            throw new ArgumentException("Collection cannot be null or empty.", paramName);
        }

        return value;
    }

    /// <summary>
    /// Throws ArgumentException if the string exceeds the maximum length.
    /// </summary>
    public static string MaxLength(
        string value,
        int maxLength,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value?.Length > maxLength)
        {
            throw new ArgumentException($"String cannot exceed {maxLength} characters.", paramName);
        }

        return value!;
    }

    /// <summary>
    /// Throws ArgumentException if the condition is false.
    /// </summary>
    public static void Against(
        bool condition,
        string message,
        [CallerArgumentExpression(nameof(condition))] string? paramName = null)
    {
        if (condition)
        {
            throw new ArgumentException(message, paramName);
        }
    }
}