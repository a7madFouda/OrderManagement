using MediatR;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Discounts;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Orders;

namespace OrderManagement.Application.Orders.Commands.UpdateOrder;

internal sealed class UpdateOrderCommandHandler(
    IRepository<Order> orders,
    IDiscountService discountService,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrderCommand>
{
    public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.Id);

        var discountStrategy = discountService.GetStrategy(request.DiscountType);
        order.Update(request.Items.ToDetails(), discountStrategy, request.DiscountValue);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
