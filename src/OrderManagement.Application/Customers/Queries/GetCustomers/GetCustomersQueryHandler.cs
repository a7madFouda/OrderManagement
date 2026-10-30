using MediatR;
using OrderManagement.Application.Customers.Specifications;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Customers;

namespace OrderManagement.Application.Customers.Queries.GetCustomers;

internal sealed class GetCustomersQueryHandler(IRepository<Customer> customers)
    : IRequestHandler<GetCustomersQuery, IReadOnlyList<CustomerDto>>
{
    public async Task<IReadOnlyList<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customerList = await customers.ListAsync(new CustomersOrderedByNameSpecification(), cancellationToken);

        return customerList.Select(customer => customer.ToDto()).ToList();
    }
}
