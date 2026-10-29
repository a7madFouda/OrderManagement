using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Customers;

namespace OrderManagement.Infrastructure.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.Name).HasMaxLength(100).IsRequired();
        builder.Property(customer => customer.Email).HasMaxLength(200).IsRequired();
        builder.Property(customer => customer.Phone).HasMaxLength(20).IsRequired();

        builder.HasIndex(customer => customer.Email).IsUnique();

        // Seed data needs a fixed date, otherwise every new migration would detect a change.
        var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new { Id = 1, Name = "Ahmed Ali", Email = "ahmed.ali@example.com", Phone = "+201001234567", CreatedAt = seededAt },
            new { Id = 2, Name = "Sara Mahmoud", Email = "sara.mahmoud@example.com", Phone = "+201112345678", CreatedAt = seededAt },
            new { Id = 3, Name = "Omar Khaled", Email = "omar.khaled@example.com", Phone = "+201223456789", CreatedAt = seededAt });
    }
}
