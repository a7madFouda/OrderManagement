using MediatR;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Orders;

namespace OrderManagement.Application.Orders.Commands.DeleteOrder;

internal sealed class DeleteOrderCommandHandler(IRepository<Order> orders, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteOrderCommand>
{
    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.Id);

        order.Delete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
