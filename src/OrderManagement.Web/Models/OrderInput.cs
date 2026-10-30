using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Web.Models;

public class OrderInput
{
    public DiscountType DiscountType { get; init; }

    [Range(0, double.MaxValue, ErrorMessage = "Discount value cannot be negative.")]
    public decimal DiscountValue { get; init; }

    [MinLength(1, ErrorMessage = "Add at least one item.")]
    public List<OrderItemInput> Items { get; init; } = [];
}
