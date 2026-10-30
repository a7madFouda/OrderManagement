using OrderManagement.Domain.Common;

namespace OrderManagement.Infrastructure.Persistence;

internal sealed class UnitOfWork(OrderManagementDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
