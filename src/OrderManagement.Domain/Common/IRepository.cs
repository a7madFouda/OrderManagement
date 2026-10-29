namespace OrderManagement.Domain.Common;

public interface IRepository<T> where T : AggregateRoot
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ListAsync(Specification<T> specification, CancellationToken cancellationToken = default);

    Task<int> CountAsync(Specification<T> specification, CancellationToken cancellationToken = default);

    void Add(T aggregate);

    void Remove(T aggregate);
}
