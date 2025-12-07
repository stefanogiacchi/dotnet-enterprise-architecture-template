using System.Linq.Expressions;

namespace ArcAI.Domain.Common;

/// <summary>
/// Defines a specification for filtering entities.
/// Implements the Specification pattern for encapsulating business rules.
/// </summary>
/// <typeparam name="T">The entity type this specification applies to.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Gets the criteria expression for filtering.
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// Gets the include expressions for eager loading.
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Gets the string-based include paths for nested eager loading.
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Gets the order by expression.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Gets the order by descending expression.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Gets the number of records to take.
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Gets the number of records to skip.
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Gets whether paging is enabled.
    /// </summary>
    bool IsPagingEnabled { get; }
}

/// <summary>
/// Base implementation of specification pattern.
/// </summary>
/// <typeparam name="T">The entity type this specification applies to.</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; private set; } = _ => true;
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int? Take { get; private set; }
    public int? Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    protected Specification() { }

    protected Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}