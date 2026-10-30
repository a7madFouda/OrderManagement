using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Domain.UnitTests.Orders.Discounts;

public sealed class PercentageDiscountStrategyTests
{
    private readonly PercentageDiscountStrategy _strategy = new();

    [Theory]
    [InlineData(200, 10, 20)]
    [InlineData(31.68, 12.56, 3.98)]
    public void CalculateDiscount_ReturnsPercentageOfSubTotalRounded(decimal subTotal, decimal percentage, decimal expected)
    {
        var discount = _strategy.CalculateDiscount(subTotal, percentage);

        Assert.Equal(expected, discount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(100.01)]
    public void Validate_PercentageOutOfRange_ReturnsError(decimal percentage)
    {
        var error = _strategy.Validate(200, percentage);

        Assert.Equal("Discount percentage must be greater than 0 and at most 100.", error);
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(100)]
    public void Validate_PercentageInRange_ReturnsNull(decimal percentage)
    {
        Assert.Null(_strategy.Validate(200, percentage));
    }
}
