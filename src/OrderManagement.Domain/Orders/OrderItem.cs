using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Orders;

public sealed class OrderItem : Entity
{
    public const int ProductNameMaxLength = 200;

    internal OrderItem(string productName, int quantity, decimal unitPrice)
    {
        ProductName = productName.Trim();
        Quantity = quantity;
        UnitPrice = Math.Round(unitPrice, 2, MidpointRounding.AwayFromZero);
    }

    public string ProductName { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal LineTotal => Quantity * UnitPrice;
}
