using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Web.Models;

public sealed class CreateOrderInput : OrderInput
{
    [Range(1, int.MaxValue, ErrorMessage = "Please select a customer.")]
    public int CustomerId { get; init; }
}
