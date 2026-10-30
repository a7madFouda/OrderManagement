namespace OrderManagement.Domain.Orders.Discounts;

public interface IDiscountStrategy
{
    DiscountType Type { get; }

    string? Validate(decimal subTotal, decimal value);

    decimal CalculateDiscount(decimal subTotal, decimal value);
}
