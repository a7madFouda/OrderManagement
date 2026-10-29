using OrderManagement.Domain.Common;

namespace OrderManagement.Infrastructure.Persistence.Specifications;

internal static class SpecificationEvaluator
{
    public static IQueryable<T> ApplyCriteria<T>(IQueryable<T> query, Specification<T> specification)
        where T : AggregateRoot
    {
        return specification.Criteria is null ? query : query.Where(specification.Criteria);
    }

    public static IQueryable<T> GetQuery<T>(IQueryable<T> query, Specification<T> specification)
        where T : AggregateRoot
    {
        query = ApplyCriteria(query, specification);

        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.Skip is not null)
        {
            query = query.Skip(specification.Skip.Value);
        }

        if (specification.Take is not null)
        {
            query = query.Take(specification.Take.Value);
        }

        return query;
    }
}
