using OrderManagement.Domain.Common;

namespace OrderManagement.Infrastructure.Persistence;

/// <summary>
/// Commits all changes tracked by the repositories of the current request in a single transaction.
/// </summary>
internal sealed class UnitOfWork(OrderManagementDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
