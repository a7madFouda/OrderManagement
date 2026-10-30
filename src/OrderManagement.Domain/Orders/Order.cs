using OrderManagement.Domain.Common;
using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Domain.Orders;

public sealed class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = [];

    // For EF Core
    private Order()
    {
    }

    internal Order(
        Customer customer,
        IReadOnlyCollection<OrderItemDetails> items,
        IDiscountStrategy discountStrategy,
        decimal discountValue)
    {
        CustomerId = customer.Id;
        ReplaceItems(items);
        ApplyDiscount(discountStrategy, discountValue);
    }

    public int CustomerId { get; private set; }

    public DiscountType DiscountType { get; private set; }

    public decimal DiscountValue { get; private set; }

    public decimal SubTotal { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal Total { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public void Update(IReadOnlyCollection<OrderItemDetails> items, IDiscountStrategy discountStrategy, decimal discountValue)
    {
        OrderRules.Validate(items, discountStrategy, discountValue);

        ReplaceItems(items);
        ApplyDiscount(discountStrategy, discountValue);
        MarkAsUpdated();
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    private void ReplaceItems(IReadOnlyCollection<OrderItemDetails> items)
    {
        _items.Clear();
        _items.AddRange(items.Select(item => new OrderItem(item.ProductName, item.Quantity, item.UnitPrice)));

        SubTotal = _items.Sum(item => item.LineTotal);
    }

    private void ApplyDiscount(IDiscountStrategy discountStrategy, decimal discountValue)
    {
        DiscountType = discountStrategy.Type;
        DiscountValue = Math.Round(discountValue, 2, MidpointRounding.AwayFromZero);
        DiscountAmount = discountStrategy.CalculateDiscount(SubTotal, DiscountValue);
        Total = SubTotal - DiscountAmount;
    }
}
