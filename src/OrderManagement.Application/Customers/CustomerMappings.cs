using OrderManagement.Domain.Customers;

namespace OrderManagement.Application.Customers;

internal static class CustomerMappings
{
    public static CustomerDto ToDto(this Customer customer) =>
        new(customer.Id, customer.Name, customer.Email, customer.Phone);
}
