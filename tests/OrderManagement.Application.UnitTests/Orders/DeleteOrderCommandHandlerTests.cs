using NSubstitute;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Orders.Commands.DeleteOrder;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.UnitTests.Orders;

public sealed class DeleteOrderCommandHandlerTests
{
    private readonly IRepository<Order> _orders = Substitute.For<IRepository<Order>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteOrderCommandHandler _handler;

    public DeleteOrderCommandHandlerTests()
    {
        _handler = new DeleteOrderCommandHandler(_orders, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ExistingOrder_SoftDeletesAndSaves()
    {
        var order = new OrderFactory().Create(
            new Customer("John Smith", "john@example.com", "01000000000"),
            [new("Laptop", 1, 1000)],
            new NoDiscountStrategy(),
            0);
        _orders.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(order);

        await _handler.Handle(new DeleteOrderCommand(1), CancellationToken.None);

        Assert.True(order.IsDeleted);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OrderNotFound_ThrowsNotFoundAndSavesNothing()
    {
        _orders.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Order?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(new DeleteOrderCommand(99), CancellationToken.None));

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
