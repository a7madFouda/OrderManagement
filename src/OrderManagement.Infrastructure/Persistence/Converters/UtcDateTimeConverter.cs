using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OrderManagement.Infrastructure.Persistence.Converters;

// datetime2 doesn't keep the DateTimeKind, so dates read from the database are marked as UTC
internal sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    value => value,
    value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
