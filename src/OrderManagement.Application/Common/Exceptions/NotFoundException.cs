namespace OrderManagement.Application.Common.Exceptions;

public sealed class NotFoundException(string resourceName, int id)
    : Exception($"{resourceName} with id {id} was not found.");
