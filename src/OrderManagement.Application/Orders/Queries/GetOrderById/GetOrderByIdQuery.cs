using MediatR;

namespace OrderManagement.Application.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(int Id) : IRequest<OrderDto>;
