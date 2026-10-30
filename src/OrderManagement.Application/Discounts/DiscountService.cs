using OrderManagement.Domain.Common;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.Discounts;

internal sealed class DiscountService(IEnumerable<IDiscountStrategy> strategies) : IDiscountService
{
    public IDiscountStrategy GetStrategy(DiscountType discountType)
    {
        return strategies.FirstOrDefault(strategy => strategy.Type == discountType)
            ?? throw new InvalidOperationException($"No discount strategy is registered for '{discountType}'.");
    }

    public DiscountResultDto Calculate(DiscountType discountType, decimal discountValue, decimal subTotal)
    {
        var strategy = GetStrategy(discountType);

        var error = strategy.Validate(subTotal, discountValue);
        if (error is not null)
        {
            throw new DomainException([error]);
        }

        var discountAmount = strategy.CalculateDiscount(subTotal, discountValue);

        return new DiscountResultDto(discountType, discountValue, subTotal, discountAmount, subTotal - discountAmount);
    }
}
