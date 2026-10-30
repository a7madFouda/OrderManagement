using OrderManagement.Domain.Common;
using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Domain.UnitTests.Orders;

public sealed class OrderTests
{
    private readonly Order _order = new OrderFactory().Create(
        new Customer("John Smith", "john@example.com", "01000000000"),
        [new("Laptop", 1, 1000), new("Mouse", 2, 25)],
        new NoDiscountStrategy(),
        0);

    [Fact]
    public void Update_ReplacesItemsAndRecalculatesTotals()
    {
        _order.Update([new("Keyboard", 2, 40)], new FixedAmountDiscountStrategy(), 10);

        var item = Assert.Single(_order.Items);
        Assert.Equal("Keyboard", item.ProductName);
        Assert.Equal(DiscountType.FixedAmount, _order.DiscountType);
        Assert.Equal(80m, _order.SubTotal);
        Assert.Equal(10m, _order.DiscountAmount);
        Assert.Equal(70m, _order.Total);
        Assert.NotNull(_order.UpdatedAt);
    }

    [Fact]
    public void Update_WithInvalidItems_LeavesOrderUnchanged()
    {
        Assert.Throws<DomainException>(() => _order.Update([], new NoDiscountStrategy(), 0));

        Assert.Equal(2, _order.Items.Count);
        Assert.Equal(1050m, _order.Total);
        Assert.Null(_order.UpdatedAt);
    }

    [Fact]
    public void Delete_MarksOrderAsDeleted()
    {
        _order.Delete();

        Assert.True(_order.IsDeleted);
        Assert.NotNull(_order.DeletedAt);
    }
}
