using ArcAI.Application.Common.Models;

namespace ArcAI.Application.Common.Extensions;

/// <summary>
/// Extension methods for creating paginated lists.
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Creates a PaginatedList from a tuple of items and total count.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    /// <param name="tuple">Tuple containing items and total count.</param>
    /// <param name="pageNumber">The current page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A PaginatedList.</returns>
    public static PaginatedList<T> ToPaginatedList<T>(
        this (IReadOnlyList<T> Items, int TotalCount) tuple,
        int pageNumber,
        int pageSize)
    {
        return new PaginatedList<T>(
            tuple.Items,
            tuple.TotalCount,
            pageNumber,
            pageSize);
    }

    /// <summary>
    /// Creates a PaginatedList from a collection.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="pageNumber">The current page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A PaginatedList.</returns>
    public static PaginatedList<T> ToPaginatedList<T>(
        this IReadOnlyList<T> source,
        int pageNumber,
        int pageSize)
    {
        return PaginatedList<T>.Create(source, pageNumber, pageSize);
    }
}