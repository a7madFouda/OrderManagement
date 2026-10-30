using MediatR;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery(
    string? Search,
    int? CustomerId,
    OrderSortBy SortBy,
    bool SortDescending,
    int Page,
    int PageSize) : IRequest<PagedResult<OrderDto>>;
