using Microsoft.AspNetCore.Mvc;
using OrderManagement.Web.Models;
using OrderManagement.Web.Services;

namespace OrderManagement.Web.Pages.Orders;

public sealed class EditModel(IOrdersApiClient ordersApi) : OrderFormModel(ordersApi)
{
    public OrderDto Order { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var order = await OrdersApi.GetOrderAsync(id, cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        Order = order;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, [FromBody] OrderInput input, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationErrors();
        }

        var result = await OrdersApi.UpdateOrderAsync(id, input, cancellationToken);

        return result.Succeeded
            ? RedirectToOrders($"Order #{id} was updated.")
            : Errors(result.Errors);
    }
}
