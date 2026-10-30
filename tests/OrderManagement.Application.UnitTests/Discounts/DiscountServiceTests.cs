using OrderManagement.Application.Discounts;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.UnitTests.Discounts;

public sealed class DiscountServiceTests
{
    private readonly DiscountService _service = new(
    [
        new NoDiscountStrategy(),
        new PercentageDiscountStrategy(),
        new FixedAmountDiscountStrategy()
    ]);

    [Theory]
    [InlineData(DiscountType.None, 0, 0, 200)]
    [InlineData(DiscountType.Percentage, 10, 20, 180)]
    [InlineData(DiscountType.FixedAmount, 50, 50, 150)]
    public void Calculate_UsesTheMatchingStrategy(
        DiscountType discountType,
        decimal discountValue,
        decimal expectedDiscount,
        decimal expectedTotal)
    {
        var result = _service.Calculate(discountType, discountValue, 200);

        Assert.Equal(expectedDiscount, result.DiscountAmount);
        Assert.Equal(expectedTotal, result.Total);
    }

    [Fact]
    public void Calculate_InvalidValue_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(
            () => _service.Calculate(DiscountType.Percentage, 150, 200));

        Assert.Equal(["Discount percentage must be greater than 0 and at most 100."], exception.Errors);
    }
}
