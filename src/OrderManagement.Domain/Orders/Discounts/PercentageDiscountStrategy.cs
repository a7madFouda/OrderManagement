namespace OrderManagement.Domain.Orders.Discounts;

public sealed class PercentageDiscountStrategy : IDiscountStrategy
{
    public DiscountType Type => DiscountType.Percentage;

    public string? Validate(decimal subTotal, decimal value)
    {
        return value is > 0 and <= 100
            ? null
            : "Discount percentage must be greater than 0 and at most 100.";
    }

    public decimal CalculateDiscount(decimal subTotal, decimal value) =>
        Math.Round(subTotal * value / 100, 2, MidpointRounding.AwayFromZero);
}
