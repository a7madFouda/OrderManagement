using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.Discounts;

public interface IDiscountService
{
    IDiscountStrategy GetStrategy(DiscountType discountType);

    DiscountResultDto Calculate(DiscountType discountType, decimal discountValue, decimal subTotal);
}
