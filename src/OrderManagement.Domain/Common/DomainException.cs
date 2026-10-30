namespace OrderManagement.Domain.Common;

public sealed class DomainException(IReadOnlyList<string> errors) : Exception(string.Join(" ", errors))
{
    public IReadOnlyList<string> Errors { get; } = errors;
}
