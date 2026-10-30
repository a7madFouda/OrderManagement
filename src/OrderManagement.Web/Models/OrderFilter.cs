namespace OrderManagement.Web.Models;

public sealed class OrderFilter
{
    public string? Search { get; init; }

    public int? CustomerId { get; init; }

    public OrderSortBy SortBy { get; init; } = OrderSortBy.Date;

    public bool SortDescending { get; init; } = true;

    public int Page { get; init; } = 1;
}
