using OrderManagement.Application.Orders;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Api.Contracts;

public sealed record CreateOrderRequest(
    int CustomerId,
    DiscountType DiscountType,
    decimal DiscountValue,
    IReadOnlyList<OrderItemInput> Items);
