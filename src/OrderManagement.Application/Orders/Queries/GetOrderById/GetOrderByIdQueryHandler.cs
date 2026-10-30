using MediatR;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Customers;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders;

namespace OrderManagement.Application.Orders.Queries.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(IRepository<Order> orders, IRepository<Customer> customers)
    : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.Id);

        var customer = await customers.GetByIdAsync(order.CustomerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), order.CustomerId);

        return order.ToDto(customer.ToDto());
    }
}
