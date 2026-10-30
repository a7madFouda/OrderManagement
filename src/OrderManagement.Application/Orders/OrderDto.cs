using OrderManagement.Application.Customers;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.Orders;

public sealed record OrderDto(
    int Id,
    CustomerDto Customer,
    DiscountType DiscountType,
    decimal DiscountValue,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal Total,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<OrderItemDto> Items);
