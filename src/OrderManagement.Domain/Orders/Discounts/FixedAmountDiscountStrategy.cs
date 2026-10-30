namespace OrderManagement.Domain.Orders.Discounts;

public sealed class FixedAmountDiscountStrategy : IDiscountStrategy
{
    public DiscountType Type => DiscountType.FixedAmount;

    public string? Validate(decimal subTotal, decimal value)
    {
        if (value <= 0)
        {
            return "Discount amount must be greater than zero.";
        }

        if (value > subTotal)
        {
            return "Discount amount cannot exceed the subtotal.";
        }

        return null;
    }

    public decimal CalculateDiscount(decimal subTotal, decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
