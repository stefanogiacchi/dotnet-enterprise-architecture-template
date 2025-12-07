using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ArcAI.Shared.Extensions;

/// <summary>
/// Extension methods for strings.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// Truncates a string to the specified length.
    /// </summary>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value))
            return value;

        if (value.Length <= maxLength)
            return value;

        return value[..(maxLength - suffix.Length)] + suffix;
    }

    /// <summary>
    /// Converts a string to slug format (URL-friendly).
    /// </summary>
    public static string ToSlug(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        // Convert to lowercase
        var slug = value.ToLowerInvariant();

        // Remove diacritics
        slug = RemoveDiacritics(slug);

        // Replace spaces with hyphens
        slug = slug.Replace(' ', '-');

        // Remove invalid characters
        slug = SlugRegex().Replace(slug, "");

        // Remove multiple hyphens
        slug = MultipleHyphensRegex().Replace(slug, "-");

        // Trim hyphens from ends
        slug = slug.Trim('-');

        return slug;
    }

    /// <summary>
    /// Converts a string to Title Case.
    /// </summary>
    public static string ToTitleCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
    }

    /// <summary>
    /// Checks if the string contains only alphanumeric characters.
    /// </summary>
    public static bool IsAlphanumeric(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        return AlphanumericRegex().IsMatch(value);
    }

    /// <summary>
    /// Removes whitespace from a string.
    /// </summary>
    public static string RemoveWhitespace(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return WhitespaceRegex().Replace(value, "");
    }

    /// <summary>
    /// Checks if a string is a valid email format.
    /// </summary>
    public static bool IsValidEmail(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        return EmailRegex().IsMatch(value);
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex(@"[^a-z0-9\-]")]
    private static partial Regex SlugRegex();

    [GeneratedRegex(@"-+")]
    private static partial Regex MultipleHyphensRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9]+$")]
    private static partial Regex AlphanumericRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}