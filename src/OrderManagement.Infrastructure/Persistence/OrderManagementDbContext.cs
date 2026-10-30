using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders;
using OrderManagement.Infrastructure.Persistence.Converters;

namespace OrderManagement.Infrastructure.Persistence;

public sealed class OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderManagementDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<UtcDateTimeConverter>();
    }
}
