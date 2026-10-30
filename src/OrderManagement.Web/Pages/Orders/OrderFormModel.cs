using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderManagement.Web.Models;
using OrderManagement.Web.Services;

namespace OrderManagement.Web.Pages.Orders;

public abstract class OrderFormModel(IOrdersApiClient ordersApi) : PageModel
{
    protected IOrdersApiClient OrdersApi { get; } = ordersApi;

    public async Task<IActionResult> OnGetDiscountAsync(
        DiscountType discountType,
        decimal discountValue,
        decimal subTotal,
        CancellationToken cancellationToken)
    {
        var result = await OrdersApi.PreviewDiscountAsync(discountType, discountValue, subTotal, cancellationToken);

        return result.Succeeded ? new JsonResult(result.Value) : Errors(result.Errors);
    }

    protected IActionResult ValidationErrors() =>
        Errors(ModelState.Values.SelectMany(entry => entry.Errors).Select(error => error.ErrorMessage));

    protected IActionResult Errors(IEnumerable<string> errors) => BadRequest(new { errors });

    protected IActionResult RedirectToOrders(string statusMessage)
    {
        TempData[nameof(IndexModel.StatusMessage)] = statusMessage;

        return new JsonResult(new { redirectUrl = Url.Page("Index") });
    }
}
