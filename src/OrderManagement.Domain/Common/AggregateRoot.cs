namespace OrderManagement.Domain.Common;

/// <summary>
/// Entry point of an aggregate. Only aggregate roots are loaded and persisted through repositories,
/// and they carry the audit dates for every change made inside the aggregate.
/// </summary>
public abstract class AggregateRoot : Entity
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    protected void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;
}
