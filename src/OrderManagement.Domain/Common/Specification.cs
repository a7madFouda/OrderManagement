using System.Linq.Expressions;

namespace OrderManagement.Domain.Common;

/// <summary>
/// Encapsulates the filtering, sorting and paging of an aggregate query.
/// </summary>
public abstract class Specification<T> where T : AggregateRoot
{
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    public Expression<Func<T, object>>? OrderBy { get; private set; }

    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    public int? Skip { get; private set; }

    public int? Take { get; private set; }

    protected void Where(Expression<Func<T, bool>> criteria) => Criteria = criteria;

    protected void ApplyOrderBy(Expression<Func<T, object>> keySelector) => OrderBy = keySelector;

    protected void ApplyOrderByDescending(Expression<Func<T, object>> keySelector) => OrderByDescending = keySelector;

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }
}
