namespace OrderManagement.Web.Models;

public sealed record OrderItemDto(int Id, string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal);
