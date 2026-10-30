using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Common;
using OrderManagement.Infrastructure.Persistence.Specifications;

namespace OrderManagement.Infrastructure.Persistence.Repositories;

internal sealed class Repository<T>(OrderManagementDbContext context) : IRepository<T>
    where T : AggregateRoot
{
    private readonly DbSet<T> _aggregates = context.Set<T>();

    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _aggregates.FirstOrDefaultAsync(aggregate => aggregate.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ListAsync(Specification<T> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator.GetQuery(_aggregates, specification).ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(Specification<T> specification, CancellationToken cancellationToken = default)
    {
        return SpecificationEvaluator.ApplyCriteria(_aggregates, specification).CountAsync(cancellationToken);
    }

    public void Add(T aggregate) => _aggregates.Add(aggregate);
}
