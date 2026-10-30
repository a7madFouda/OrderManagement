using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.Discounts;

public sealed record DiscountResultDto(
    DiscountType DiscountType,
    decimal DiscountValue,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal Total);
