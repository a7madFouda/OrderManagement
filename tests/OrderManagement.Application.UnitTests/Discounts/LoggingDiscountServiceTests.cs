using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using OrderManagement.Application.Discounts;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.UnitTests.Discounts;

public sealed class LoggingDiscountServiceTests
{
    [Fact]
    public void Calculate_ReturnsTheResultOfTheInnerService()
    {
        var inner = Substitute.For<IDiscountService>();
        var expected = new DiscountResultDto(DiscountType.Percentage, 10, 200, 20, 180);
        inner.Calculate(DiscountType.Percentage, 10, 200).Returns(expected);

        var service = new LoggingDiscountService(inner, NullLogger<LoggingDiscountService>.Instance);

        var result = service.Calculate(DiscountType.Percentage, 10, 200);

        Assert.Same(expected, result);
        inner.Received(1).Calculate(DiscountType.Percentage, 10, 200);
    }
}
