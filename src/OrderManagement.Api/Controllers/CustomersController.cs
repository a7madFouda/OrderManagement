using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Customers;
using OrderManagement.Application.Customers.Queries.GetCustomers;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetCustomers(CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(new GetCustomersQuery(), cancellationToken));
    }
}
