using OrderManagement.Application.Orders;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Api.Contracts;

public sealed record UpdateOrderRequest(
    DiscountType DiscountType,
    decimal DiscountValue,
    IReadOnlyList<OrderItemInput> Items);
