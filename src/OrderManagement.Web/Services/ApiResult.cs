namespace OrderManagement.Web.Services;

public sealed record ApiResult<T>(T? Value, IReadOnlyList<string> Errors)
{
    public bool Succeeded => Errors.Count == 0;

    public static ApiResult<T> Success(T value) => new(value, []);

    public static ApiResult<T> Failure(IReadOnlyList<string> errors) => new(default, errors);
}
