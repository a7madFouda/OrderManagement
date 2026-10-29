using OrderManagement.Domain.Common;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Domain.Orders;

public sealed class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = [];

    // Required by EF Core.
    private Order()
    {
    }

    internal Order(int customerId)
    {
        CustomerId = customerId;
    }

    public int CustomerId { get; private set; }

    public DiscountType DiscountType { get; private set; }

    public decimal SubTotal { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal Total { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    internal void AddItem(string productName, int quantity, decimal unitPrice)
    {
        _items.Add(new OrderItem(productName, quantity, unitPrice));

        SubTotal = _items.Sum(item => item.LineTotal);
        Total = SubTotal - DiscountAmount;
    }
}
