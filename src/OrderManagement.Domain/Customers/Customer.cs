using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Customers;

public sealed class Customer : AggregateRoot
{
    public Customer(string name, string email, string phone)
    {
        Name = name;
        Email = email;
        Phone = phone;
    }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public string Phone { get; private set; }
}
