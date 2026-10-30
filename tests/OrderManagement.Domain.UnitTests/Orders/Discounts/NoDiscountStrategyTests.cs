using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Domain.UnitTests.Orders.Discounts;

public sealed class NoDiscountStrategyTests
{
    [Fact]
    public void IgnoresTheValueAndReturnsZero()
    {
        var strategy = new NoDiscountStrategy();

        Assert.Null(strategy.Validate(200, -1));
        Assert.Equal(0m, strategy.CalculateDiscount(200, 50));
    }
}
