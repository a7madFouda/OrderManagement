namespace OrderManagement.Domain.Orders;

public sealed record OrderItemDetails(string ProductName, int Quantity, decimal UnitPrice);
