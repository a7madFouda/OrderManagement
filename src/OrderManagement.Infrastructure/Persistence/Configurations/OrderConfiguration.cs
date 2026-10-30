using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Customers;
using OrderManagement.Domain.Orders;

namespace OrderManagement.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(order => order.Id);

        builder.HasQueryFilter(order => !order.IsDeleted);

        builder.Property(order => order.DiscountType).HasConversion<string>().HasMaxLength(20);
        builder.Property(order => order.DiscountValue).HasPrecision(18, 2);
        builder.Property(order => order.SubTotal).HasPrecision(18, 2);
        builder.Property(order => order.DiscountAmount).HasPrecision(18, 2);
        builder.Property(order => order.Total).HasPrecision(18, 2);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(order => order.Items, items =>
        {
            items.ToTable("OrderItems");
            items.WithOwner().HasForeignKey("OrderId");
            items.HasKey(item => item.Id);

            items.Property(item => item.ProductName).HasMaxLength(OrderItem.ProductNameMaxLength).IsRequired();
            items.Property(item => item.UnitPrice).HasPrecision(18, 2);
        });

        builder.Navigation(order => order.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
