using OrderManagement.Application.Customers;
using OrderManagement.Domain.Orders;

namespace OrderManagement.Application.Orders;

internal static class OrderMappings
{
    public static OrderDto ToDto(this Order order, CustomerDto customer) =>
        new(
            order.Id,
            customer,
            order.DiscountType,
            order.DiscountValue,
            order.SubTotal,
            order.DiscountAmount,
            order.Total,
            order.CreatedAt,
            order.UpdatedAt,
            order.Items
                .Select(item => new OrderItemDto(item.Id, item.ProductName, item.Quantity, item.UnitPrice, item.LineTotal))
                .ToList());

    public static IReadOnlyCollection<OrderItemDetails> ToDetails(this IEnumerable<OrderItemInput>? items) =>
        (items ?? [])
            .Select(item => new OrderItemDetails(item.ProductName, item.Quantity, item.UnitPrice))
            .ToList();
}
