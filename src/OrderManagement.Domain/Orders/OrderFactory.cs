using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Domain.Orders;

public sealed class OrderFactory : IOrderFactory
{
    public Order Create(
        Customer customer,
        IReadOnlyCollection<OrderItemDetails> items,
        IDiscountStrategy discountStrategy,
        decimal discountValue)
    {
        OrderRules.Validate(items, discountStrategy, discountValue);

        return new Order(customer, items, discountStrategy, discountValue);
    }
}
