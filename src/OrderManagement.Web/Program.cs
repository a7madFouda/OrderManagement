using System.Text.Json.Serialization;
using OrderManagement.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorPages(options => options.Conventions.AddPageRoute("/Orders/Index", ""))
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddHttpClient<IOrdersApiClient, OrdersApiClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["OrdersApi:BaseUrl"]!));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
