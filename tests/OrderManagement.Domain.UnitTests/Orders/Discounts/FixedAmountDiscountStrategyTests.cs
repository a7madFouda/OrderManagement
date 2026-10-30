using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Domain.UnitTests.Orders.Discounts;

public sealed class FixedAmountDiscountStrategyTests
{
    private readonly FixedAmountDiscountStrategy _strategy = new();

    [Fact]
    public void CalculateDiscount_ReturnsTheAmount()
    {
        Assert.Equal(50m, _strategy.CalculateDiscount(200, 50));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Validate_AmountNotPositive_ReturnsError(decimal amount)
    {
        var error = _strategy.Validate(200, amount);

        Assert.Equal("Discount amount must be greater than zero.", error);
    }

    [Fact]
    public void Validate_AmountGreaterThanSubTotal_ReturnsError()
    {
        var error = _strategy.Validate(100, 100.01m);

        Assert.Equal("Discount amount cannot exceed the subtotal.", error);
    }

    [Fact]
    public void Validate_AmountEqualToSubTotal_ReturnsNull()
    {
        Assert.Null(_strategy.Validate(100, 100));
    }
}
