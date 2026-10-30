using System.Text.Json.Serialization;
using OrderManagement.Api.ExceptionHandling;
using OrderManagement.Application;
using OrderManagement.Infrastructure;
using OrderManagement.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    // Let the domain rules report all validation errors together
    .AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    await app.Services.ApplyMigrationsAsync();

    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Order Management API"));
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
