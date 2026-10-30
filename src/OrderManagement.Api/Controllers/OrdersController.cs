using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Contracts;
using OrderManagement.Application.Common;
using OrderManagement.Application.Discounts;
using OrderManagement.Application.Orders;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Application.Orders.Commands.DeleteOrder;
using OrderManagement.Application.Orders.Commands.UpdateOrder;
using OrderManagement.Application.Orders.Queries.GetDiscountPreview;
using OrderManagement.Application.Orders.Queries.GetOrderById;
using OrderManagement.Application.Orders.Queries.GetOrders;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetOrders(
        [FromQuery] string? search,
        [FromQuery] int? customerId,
        [FromQuery] OrderSortBy sortBy = OrderSortBy.Date,
        [FromQuery] bool sortDescending = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetOrdersQuery(search, customerId, sortBy, sortDescending, page, pageSize);

        return Ok(await sender.Send(query, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id, CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(new GetOrderByIdQuery(id), cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateOrder(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(request.CustomerId, request.DiscountType, request.DiscountValue, request.Items);
        var orderId = await sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetOrder), new { id = orderId }, orderId);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateOrder(int id, UpdateOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateOrderCommand(id, request.DiscountType, request.DiscountValue, request.Items);
        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteOrderCommand(id), cancellationToken);

        return NoContent();
    }

    [HttpGet("discount-preview")]
    public async Task<ActionResult<DiscountResultDto>> GetDiscountPreview(
        [FromQuery] DiscountType discountType,
        [FromQuery] decimal discountValue,
        [FromQuery, Range(0, double.MaxValue, ErrorMessage = "Subtotal cannot be negative.")] decimal subTotal,
        CancellationToken cancellationToken)
    {
        var query = new GetDiscountPreviewQuery(discountType, discountValue, subTotal);

        return Ok(await sender.Send(query, cancellationToken));
    }
}
