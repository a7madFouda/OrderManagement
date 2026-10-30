using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Discounts;
using OrderManagement.Domain.Orders;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<IOrderFactory, OrderFactory>();

        services.AddSingleton<IDiscountStrategy, NoDiscountStrategy>();
        services.AddSingleton<IDiscountStrategy, PercentageDiscountStrategy>();
        services.AddSingleton<IDiscountStrategy, FixedAmountDiscountStrategy>();

        services.AddSingleton<DiscountService>();
        services.AddSingleton<IDiscountService>(provider => new LoggingDiscountService(
            provider.GetRequiredService<DiscountService>(),
            provider.GetRequiredService<ILogger<LoggingDiscountService>>()));

        return services;
    }
}
