using MediatR;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.Orders.Commands.UpdateOrder;

public sealed record UpdateOrderCommand(
    int Id,
    DiscountType DiscountType,
    decimal DiscountValue,
    IReadOnlyList<OrderItemInput> Items) : IRequest;
