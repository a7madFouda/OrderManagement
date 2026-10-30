using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using OrderManagement.Web.Models;

namespace OrderManagement.Web.Services;

internal sealed class OrdersApiClient(HttpClient httpClient) : IOrdersApiClient
{
    private const int PageSize = 10;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<PagedResult<OrderDto>> GetOrdersAsync(OrderFilter filter, CancellationToken cancellationToken)
    {
        var url = QueryHelpers.AddQueryString("api/orders", new Dictionary<string, string?>
        {
            ["search"] = filter.Search,
            ["customerId"] = filter.CustomerId?.ToString(CultureInfo.InvariantCulture),
            ["sortBy"] = filter.SortBy.ToString(),
            ["sortDescending"] = filter.SortDescending.ToString(),
            ["page"] = filter.Page.ToString(CultureInfo.InvariantCulture),
            ["pageSize"] = PageSize.ToString(CultureInfo.InvariantCulture)
        });

        return await httpClient.GetFromJsonAsync<PagedResult<OrderDto>>(url, JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("The orders API returned an empty response.");
    }

    public async Task<OrderDto?> GetOrderAsync(int id, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync($"api/orders/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OrderDto>(JsonOptions, cancellationToken);
    }

    public async Task<IReadOnlyList<CustomerDto>> GetCustomersAsync(CancellationToken cancellationToken)
    {
        return await httpClient.GetFromJsonAsync<List<CustomerDto>>("api/customers", JsonOptions, cancellationToken) ?? [];
    }

    public async Task<ApiResult<DiscountResult>> PreviewDiscountAsync(
        DiscountType discountType,
        decimal discountValue,
        decimal subTotal,
        CancellationToken cancellationToken)
    {
        var url = QueryHelpers.AddQueryString("api/orders/discount-preview", new Dictionary<string, string?>
        {
            ["discountType"] = discountType.ToString(),
            ["discountValue"] = discountValue.ToString(CultureInfo.InvariantCulture),
            ["subTotal"] = subTotal.ToString(CultureInfo.InvariantCulture)
        });

        using var response = await httpClient.GetAsync(url, cancellationToken);

        return await ReadResultAsync<DiscountResult>(response, cancellationToken);
    }

    public async Task<ApiResult<int>> CreateOrderAsync(CreateOrderInput input, CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsJsonAsync("api/orders", input, JsonOptions, cancellationToken);

        return await ReadResultAsync<int>(response, cancellationToken);
    }

    public async Task<ApiResult<int>> UpdateOrderAsync(int id, OrderInput input, CancellationToken cancellationToken)
    {
        using var response = await httpClient.PutAsJsonAsync($"api/orders/{id}", input, JsonOptions, cancellationToken);

        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
        {
            return ApiResult<int>.Failure(await ReadErrorsAsync(response, cancellationToken));
        }

        response.EnsureSuccessStatusCode();

        return ApiResult<int>.Success(id);
    }

    public async Task<bool> DeleteOrderAsync(int id, CancellationToken cancellationToken)
    {
        using var response = await httpClient.DeleteAsync($"api/orders/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }

    private static async Task<ApiResult<T>> ReadResultAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
        {
            return ApiResult<T>.Failure(await ReadErrorsAsync(response, cancellationToken));
        }

        response.EnsureSuccessStatusCode();

        var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);

        return ApiResult<T>.Success(value!);
    }

    // Business rule errors come as a list, framework validation errors as a dictionary per field
    private static async Task<IReadOnlyList<string>> ReadErrorsAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);

        if (!problem.TryGetProperty("errors", out var errors))
        {
            return [problem.TryGetProperty("detail", out var detail) ? detail.GetString()! : "The request failed."];
        }

        return errors.ValueKind == JsonValueKind.Array
            ? errors.EnumerateArray().Select(error => error.GetString()!).ToList()
            : errors.EnumerateObject().SelectMany(field => field.Value.EnumerateArray()).Select(error => error.GetString()!).ToList();
    }
}
