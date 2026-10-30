namespace OrderManagement.Application.Orders;

public sealed record OrderItemDto(int Id, string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal);
