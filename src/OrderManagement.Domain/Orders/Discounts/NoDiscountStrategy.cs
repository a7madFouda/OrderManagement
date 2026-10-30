namespace OrderManagement.Domain.Orders.Discounts;

public sealed class NoDiscountStrategy : IDiscountStrategy
{
    public DiscountType Type => DiscountType.None;

    public string? Validate(decimal subTotal, decimal value) => null;

    public decimal CalculateDiscount(decimal subTotal, decimal value) => 0m;
}
