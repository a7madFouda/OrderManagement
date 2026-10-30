using System.Linq.Expressions;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Orders;

namespace OrderManagement.Application.Orders.Specifications;

public sealed class OrdersFilterSpecification : Specification<Order>
{
    public OrdersFilterSpecification(
        string? search,
        int? customerId,
        OrderSortBy sortBy,
        bool sortDescending,
        int page,
        int pageSize)
    {
        var searchTerm = search?.Trim();
        var hasSearchTerm = !string.IsNullOrEmpty(searchTerm);

        Where(order =>
            (!hasSearchTerm || order.Items.Any(item => item.ProductName.Contains(searchTerm!))) &&
            (customerId == null || order.CustomerId == customerId));

        Expression<Func<Order, object>> sortKey = sortBy switch
        {
            OrderSortBy.Total => order => order.Total,
            _ => order => order.CreatedAt
        };

        if (sortDescending)
        {
            ApplyOrderByDescending(sortKey);
        }
        else
        {
            ApplyOrderBy(sortKey);
        }

        ApplyPaging((page - 1) * pageSize, pageSize);
    }
}
