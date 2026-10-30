using MediatR;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Discounts;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders;

namespace OrderManagement.Application.Orders.Commands.CreateOrder;

internal sealed class CreateOrderCommandHandler(
    IRepository<Order> orders,
    IRepository<Customer> customers,
    IOrderFactory orderFactory,
    IDiscountService discountService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, int>
{
    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await customers.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), request.CustomerId);

        var discountStrategy = discountService.GetStrategy(request.DiscountType);
        var order = orderFactory.Create(customer, request.Items.ToDetails(), discountStrategy, request.DiscountValue);

        orders.Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
