namespace OrderManagement.Web.Models;

public sealed record DiscountResult(
    DiscountType DiscountType,
    decimal DiscountValue,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal Total);
