using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Customers;
using OrderManagement.Application.Customers.Specifications;
using OrderManagement.Application.Orders.Specifications;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

internal sealed class GetOrdersQueryHandler(IRepository<Order> orders, IRepository<Customer> customers)
    : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private const int MaxPageSize = 50;

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        var specification = new OrdersFilterSpecification(
            request.Search, request.CustomerId, request.SortBy, request.SortDescending, page, pageSize);

        var orderList = await orders.ListAsync(specification, cancellationToken);
        var totalCount = await orders.CountAsync(specification, cancellationToken);

        var customerIds = orderList.Select(order => order.CustomerId).Distinct().ToList();
        var customerList = await customers.ListAsync(new CustomersByIdsSpecification(customerIds), cancellationToken);
        var customersById = customerList.ToDictionary(customer => customer.Id, customer => customer.ToDto());

        var items = orderList.Select(order => order.ToDto(customersById[order.CustomerId])).ToList();

        return new PagedResult<OrderDto>(items, totalCount, page, pageSize);
    }
}
