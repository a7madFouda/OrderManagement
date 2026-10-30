using OrderManagement.Web.Models;

namespace OrderManagement.Web.Services;

public interface IOrdersApiClient
{
    Task<PagedResult<OrderDto>> GetOrdersAsync(OrderFilter filter, CancellationToken cancellationToken);

    Task<OrderDto?> GetOrderAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<CustomerDto>> GetCustomersAsync(CancellationToken cancellationToken);

    Task<ApiResult<DiscountResult>> PreviewDiscountAsync(
        DiscountType discountType,
        decimal discountValue,
        decimal subTotal,
        CancellationToken cancellationToken);

    Task<ApiResult<int>> CreateOrderAsync(CreateOrderInput input, CancellationToken cancellationToken);

    Task<ApiResult<int>> UpdateOrderAsync(int id, OrderInput input, CancellationToken cancellationToken);

    Task<bool> DeleteOrderAsync(int id, CancellationToken cancellationToken);
}
