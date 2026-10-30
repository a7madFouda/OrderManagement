using OrderManagement.Domain.Common;
using OrderManagement.Domain.Customers;

namespace OrderManagement.Application.Customers.Specifications;

public sealed class CustomersByIdsSpecification : Specification<Customer>
{
    public CustomersByIdsSpecification(IReadOnlyCollection<int> ids)
    {
        Where(customer => ids.Contains(customer.Id));
    }
}
