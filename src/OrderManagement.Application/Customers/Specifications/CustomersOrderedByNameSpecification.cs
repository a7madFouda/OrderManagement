using OrderManagement.Domain.Common;
using OrderManagement.Domain.Customers;

namespace OrderManagement.Application.Customers.Specifications;

public sealed class CustomersOrderedByNameSpecification : Specification<Customer>
{
    public CustomersOrderedByNameSpecification()
    {
        ApplyOrderBy(customer => customer.Name);
    }
}
