using NSubstitute;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Discounts;
using OrderManagement.Application.Orders;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.UnitTests.Orders;

public sealed class CreateOrderCommandHandlerTests
{
    private readonly IRepository<Order> _orders = Substitute.For<IRepository<Order>>();
    private readonly IRepository<Customer> _customers = Substitute.For<IRepository<Customer>>();
    private readonly IDiscountService _discountService = Substitute.For<IDiscountService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _handler = new CreateOrderCommandHandler(_orders, _customers, new OrderFactory(), _discountService, _unitOfWork);

        _discountService.GetStrategy(DiscountType.Percentage).Returns(new PercentageDiscountStrategy());
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsOrderWithTwoItemsAndSavesOnce()
    {
        _customers.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Customer("John Smith", "john@example.com", "01000000000"));

        var command = new CreateOrderCommand(
            1,
            DiscountType.Percentage,
            10,
            [new OrderItemInput("Laptop", 1, 1000), new OrderItemInput("Mouse", 2, 25)]);

        await _handler.Handle(command, CancellationToken.None);

        _orders.Received(1).Add(Arg.Is<Order>(order => order.Items.Count == 2 && order.Total == 945m));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ThrowsNotFoundAndSavesNothing()
    {
        _customers.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var command = new CreateOrderCommand(99, DiscountType.None, 0, [new OrderItemInput("Laptop", 1, 1000)]);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        _orders.DidNotReceive().Add(Arg.Any<Order>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidItems_ThrowsDomainExceptionAndSavesNothing()
    {
        _customers.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Customer("John Smith", "john@example.com", "01000000000"));

        var command = new CreateOrderCommand(1, DiscountType.Percentage, 10, []);

        await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(command, CancellationToken.None));

        _orders.DidNotReceive().Add(Arg.Any<Order>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
