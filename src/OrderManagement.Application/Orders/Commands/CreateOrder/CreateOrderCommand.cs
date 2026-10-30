using MediatR;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    int CustomerId,
    DiscountType DiscountType,
    decimal DiscountValue,
    IReadOnlyList<OrderItemInput> Items) : IRequest<int>;
