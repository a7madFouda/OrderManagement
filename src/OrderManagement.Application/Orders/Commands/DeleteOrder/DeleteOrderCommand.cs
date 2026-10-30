using MediatR;

namespace OrderManagement.Application.Orders.Commands.DeleteOrder;

public sealed record DeleteOrderCommand(int Id) : IRequest;
