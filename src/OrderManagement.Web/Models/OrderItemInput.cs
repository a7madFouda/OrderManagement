using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Web.Models;

public sealed class OrderItemInput
{
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(200, ErrorMessage = "Product name must not exceed 200 characters.")]
    public string ProductName { get; init; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; init; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero.")]
    public decimal UnitPrice { get; init; }
}
