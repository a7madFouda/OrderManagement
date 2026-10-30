using OrderManagement.Domain.Common;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Domain.Orders;

internal static class OrderRules
{
    public static void Validate(
        IReadOnlyCollection<OrderItemDetails> items,
        IDiscountStrategy discountStrategy,
        decimal discountValue)
    {
        var errors = ValidateItems(items);

        var subTotal = items.Sum(item => item.Quantity * item.UnitPrice);
        var discountError = discountStrategy.Validate(subTotal, discountValue);

        if (discountError is not null)
        {
            errors.Add(discountError);
        }

        if (errors.Count > 0)
        {
            throw new DomainException(errors);
        }
    }

    private static List<string> ValidateItems(IReadOnlyCollection<OrderItemDetails> items)
    {
        var errors = new List<string>();

        if (items.Count == 0)
        {
            errors.Add("An order must contain at least one item.");
        }

        foreach (var (index, item) in items.Index())
        {
            var position = index + 1;

            if (string.IsNullOrWhiteSpace(item.ProductName))
            {
                errors.Add($"Item {position}: product name is required.");
            }
            else if (item.ProductName.Trim().Length > OrderItem.ProductNameMaxLength)
            {
                errors.Add($"Item {position}: product name must not exceed {OrderItem.ProductNameMaxLength} characters.");
            }

            if (item.Quantity <= 0)
            {
                errors.Add($"Item {position}: quantity must be greater than zero.");
            }

            if (item.UnitPrice <= 0)
            {
                errors.Add($"Item {position}: unit price must be greater than zero.");
            }
        }

        var duplicateProducts = items
            .Where(item => !string.IsNullOrWhiteSpace(item.ProductName))
            .GroupBy(item => item.ProductName.Trim(), StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => $"Product '{group.Key}' is added more than once.");

        errors.AddRange(duplicateProducts);

        return errors;
    }
}
