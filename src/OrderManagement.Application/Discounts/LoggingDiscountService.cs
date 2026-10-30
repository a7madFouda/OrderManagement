using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.Discounts;

internal sealed class LoggingDiscountService(IDiscountService inner, ILogger<LoggingDiscountService> logger) : IDiscountService
{
    public IDiscountStrategy GetStrategy(DiscountType discountType)
    {
        var strategy = inner.GetStrategy(discountType);

        logger.LogInformation("Using {Strategy} for discount type {DiscountType}", strategy.GetType().Name, discountType);

        return strategy;
    }

    public DiscountResultDto Calculate(DiscountType discountType, decimal discountValue, decimal subTotal)
    {
        logger.LogInformation(
            "Calculating {DiscountType} discount with value {DiscountValue} for subtotal {SubTotal}",
            discountType, discountValue, subTotal);

        var result = inner.Calculate(discountType, discountValue, subTotal);

        logger.LogInformation(
            "Calculated {DiscountType} discount of {DiscountAmount}, total {Total}",
            discountType, result.DiscountAmount, result.Total);

        return result;
    }
}
