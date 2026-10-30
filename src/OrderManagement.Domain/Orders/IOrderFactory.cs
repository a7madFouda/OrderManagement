using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Domain.Orders;

public interface IOrderFactory
{
    Order Create(
        Customer customer,
        IReadOnlyCollection<OrderItemDetails> items,
        IDiscountStrategy discountStrategy,
        decimal discountValue);
}
