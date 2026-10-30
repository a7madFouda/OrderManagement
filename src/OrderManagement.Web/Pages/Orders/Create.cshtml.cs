using Microsoft.AspNetCore.Mvc;
using OrderManagement.Web.Models;
using OrderManagement.Web.Services;

namespace OrderManagement.Web.Pages.Orders;

public sealed class CreateModel(IOrdersApiClient ordersApi) : OrderFormModel(ordersApi)
{
    public IReadOnlyList<CustomerDto> Customers { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Customers = await OrdersApi.GetCustomersAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync([FromBody] CreateOrderInput input, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationErrors();
        }

        var result = await OrdersApi.CreateOrderAsync(input, cancellationToken);

        return result.Succeeded
            ? RedirectToOrders($"Order #{result.Value} was created.")
            : Errors(result.Errors);
    }
}
