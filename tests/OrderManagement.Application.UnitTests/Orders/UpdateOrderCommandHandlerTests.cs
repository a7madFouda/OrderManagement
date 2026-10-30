using NSubstitute;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Discounts;
using OrderManagement.Application.Orders;
using OrderManagement.Application.Orders.Commands.UpdateOrder;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.UnitTests.Orders;

public sealed class UpdateOrderCommandHandlerTests
{
    private readonly IRepository<Order> _orders = Substitute.For<IRepository<Order>>();
    private readonly IDiscountService _discountService = Substitute.For<IDiscountService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateOrderCommandHandler _handler;

    public UpdateOrderCommandHandlerTests()
    {
        _handler = new UpdateOrderCommandHandler(_orders, _discountService, _unitOfWork);

        _discountService.GetStrategy(DiscountType.FixedAmount).Returns(new FixedAmountDiscountStrategy());
    }

    [Fact]
    public async Task Handle_ExistingOrder_UpdatesItemsAndSaves()
    {
        var order = new OrderFactory().Create(
            new Customer("John Smith", "john@example.com", "01000000000"),
            [new("Laptop", 1, 1000)],
            new NoDiscountStrategy(),
            0);
        _orders.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(order);

        var command = new UpdateOrderCommand(1, DiscountType.FixedAmount, 10, [new OrderItemInput("Keyboard", 2, 40)]);

        await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("Keyboard", Assert.Single(order.Items).ProductName);
        Assert.Equal(70m, order.Total);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OrderNotFound_ThrowsNotFoundAndSavesNothing()
    {
        _orders.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Order?)null);

        var command = new UpdateOrderCommand(99, DiscountType.FixedAmount, 10, [new OrderItemInput("Keyboard", 2, 40)]);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
