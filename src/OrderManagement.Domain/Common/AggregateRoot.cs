namespace OrderManagement.Domain.Common;

public abstract class AggregateRoot : Entity
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    protected void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;
}
