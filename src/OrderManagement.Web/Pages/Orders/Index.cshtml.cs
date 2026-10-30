using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderManagement.Web.Models;
using OrderManagement.Web.Services;

namespace OrderManagement.Web.Pages.Orders;

public sealed class IndexModel(IOrdersApiClient ordersApi) : PageModel
{
    public IReadOnlyList<CustomerDto> Customers { get; private set; } = [];

    public PagedResult<OrderDto> Orders { get; private set; } = null!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Customers = await ordersApi.GetCustomersAsync(cancellationToken);
        Orders = await ordersApi.GetOrdersAsync(new OrderFilter(), cancellationToken);
    }

    public async Task<PartialViewResult> OnGetListAsync([FromQuery] OrderFilter filter, CancellationToken cancellationToken)
    {
        var orders = await ordersApi.GetOrdersAsync(filter, cancellationToken);

        return Partial("_OrdersTable", orders);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var deleted = await ordersApi.DeleteOrderAsync(id, cancellationToken);

        return deleted ? new NoContentResult() : NotFound();
    }
}
