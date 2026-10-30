using OrderManagement.Domain.Common;
using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Domain.UnitTests.Orders;

public sealed class OrderFactoryTests
{
    private readonly OrderFactory _factory = new();
    private readonly Customer _customer = new("John Smith", "john@example.com", "01000000000");

    [Fact]
    public void Create_WithTwoItems_CalculatesTotals()
    {
        OrderItemDetails[] items =
        [
            new("Laptop", 1, 1000),
            new("Mouse", 2, 25)
        ];

        var order = _factory.Create(_customer, items, new PercentageDiscountStrategy(), 10);

        Assert.Equal(2, order.Items.Count);
        Assert.Equal(DiscountType.Percentage, order.DiscountType);
        Assert.Equal(1050m, order.SubTotal);
        Assert.Equal(105m, order.DiscountAmount);
        Assert.Equal(945m, order.Total);
    }

    [Fact]
    public void Create_RoundsUnitPriceToTwoDecimals()
    {
        var order = _factory.Create(_customer, [new("Cable", 3, 10.555m)], new NoDiscountStrategy(), 0);

        Assert.Equal(10.56m, order.Items.Single().UnitPrice);
        Assert.Equal(31.68m, order.SubTotal);
    }

    [Fact]
    public void Create_WithoutItems_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(
            () => _factory.Create(_customer, [], new NoDiscountStrategy(), 0));

        Assert.Equal(["An order must contain at least one item."], exception.Errors);
    }

    [Fact]
    public void Create_WithInvalidItem_ReturnsAllErrorsTogether()
    {
        var exception = Assert.Throws<DomainException>(
            () => _factory.Create(_customer, [new("", 0, 0)], new NoDiscountStrategy(), 0));

        string[] expectedErrors =
        [
            "Item 1: product name is required.",
            "Item 1: quantity must be greater than zero.",
            "Item 1: unit price must be greater than zero."
        ];
        Assert.Equal(expectedErrors, exception.Errors);
    }

    [Fact]
    public void Create_WithDuplicateProducts_ThrowsDomainException()
    {
        OrderItemDetails[] items =
        [
            new("Laptop", 1, 1000),
            new(" laptop ", 1, 1000)
        ];

        var exception = Assert.Throws<DomainException>(
            () => _factory.Create(_customer, items, new NoDiscountStrategy(), 0));

        Assert.Equal(["Product 'Laptop' is added more than once."], exception.Errors);
    }

    [Fact]
    public void Create_WithFixedDiscountGreaterThanSubTotal_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(
            () => _factory.Create(_customer, [new("Mouse", 2, 25)], new FixedAmountDiscountStrategy(), 60));

        Assert.Equal(["Discount amount cannot exceed the subtotal."], exception.Errors);
    }
}
