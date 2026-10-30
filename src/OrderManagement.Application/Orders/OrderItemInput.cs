namespace OrderManagement.Application.Orders;

public sealed record OrderItemInput(string ProductName, int Quantity, decimal UnitPrice);
