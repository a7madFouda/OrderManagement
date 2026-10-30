using MediatR;

namespace OrderManagement.Application.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery : IRequest<IReadOnlyList<CustomerDto>>;
