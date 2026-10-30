# Order Management

An order management module built with **ASP.NET Core 10**:

- **OrderManagement.Api**: a REST API built with Clean Architecture and DDD.
- **OrderManagement.Web**: a Razor Pages front end that consumes the API.

## Tech stack

- .NET 10 / ASP.NET Core 10 (Web API + Razor Pages)
- Entity Framework Core 10 with SQL Server (LocalDB by default)
- MediatR 12.5 (the last Apache-licensed version)
- Bootstrap 5, vanilla JavaScript (ES6 modules + Fetch API, no jQuery)

## Solution structure

```
src/
├── OrderManagement.Domain          # Aggregates, business rules, discount strategies, repository/UoW abstractions
├── OrderManagement.Application     # Commands & queries (MediatR), specifications, application services, DTOs
├── OrderManagement.Infrastructure  # EF Core DbContext, configurations, migrations, repository, unit of work
├── OrderManagement.Api             # Controllers, request contracts, exception handling (composition root)
└── OrderManagement.Web             # Razor Pages UI, typed HttpClient for the API, ES6 modules
```

Dependencies point inward: `Api → Infrastructure → Application → Domain`. The Web project depends on none of them and talks to the API over HTTP only.

## Design patterns

| Pattern | Where | How it is used |
|---|---|---|
| **Repository** | `Domain/Common/IRepository.cs`, `Infrastructure/Persistence/Repositories/Repository.cs` | One generic repository, constrained to `AggregateRoot`, so only aggregate roots can be persisted. |
| **Unit of Work** | `Domain/Common/IUnitOfWork.cs`, `Infrastructure/Persistence/UnitOfWork.cs` | Repositories never save. Each handler commits once, so an order and its items are written in one transaction. |
| **Mediator (MediatR)** | `Application/Orders/Commands`, `Application/Orders/Queries`, `Application/Customers/Queries` | Every command and query has its own handler. Controllers only depend on `ISender`. |
| **Specification** | `Domain/Common/Specification.cs`, `Infrastructure/Persistence/Specifications/SpecificationEvaluator.cs`, `Application/Orders/Specifications/OrdersFilterSpecification.cs` | Filtering (product search, customer), sorting (date, total) and paging are encapsulated in specification classes. |
| **Factory** | `Domain/Orders/OrderFactory.cs` | The only way to create an `Order`. It validates the order rules (`OrderRules`) before building it. |
| **Decorator** | `Application/Discounts/LoggingDiscountService.cs` | Wraps `DiscountService` behind the same `IDiscountService` interface to add logging without changing it. It is registered manually in `Application/DependencyInjection.cs`. |
| **Strategy** | `Domain/Orders/Discounts/*Strategy.cs` | `NoDiscount`, `Percentage` and `FixedAmount` implement `IDiscountStrategy`. `DiscountService` picks one at runtime. |

## Domain model and decisions

- **Aggregates.** `Order` is an aggregate root that owns its `OrderItem` entities, and `Customer` is a separate aggregate root.
  - The task lists `OrderItem` as an aggregate root. It is modelled as an entity *inside* the `Order` aggregate because an item has no life outside its order, and the order's totals and rules depend on its items.
  - Keeping the items inside the aggregate means they can only change through `Order`, so the rules are always enforced. EF Core maps them with `OwnsMany`.
- **References between aggregates.** `Order` keeps only `CustomerId`, and an order's customer cannot change after creation. The query handlers add the customer's details to the response.
- **Totals.** `SubTotal`, `DiscountAmount` and `Total` are always recalculated together inside `Order`, so they can't get out of sync.
- **Validation** happens on three levels:
  - **Browser:** HTML5 constraints plus a few JavaScript checks.
  - **Razor Pages server:** DataAnnotations and `ModelState`.
  - **Domain:** `OrderRules` and the strategies. The API returns every broken rule in one response.
- **Soft delete.** Deleting an order sets `IsDeleted` and `DeletedAt`. An EF Core global query filter hides deleted orders.
- **Audit.** Aggregate roots carry `CreatedAt` and `UpdatedAt`.

### Discount rules

| Type | Value | Rule |
|---|---|---|
| `None` | ignored | — |
| `Percentage` | percent of the subtotal | greater than 0 and at most 100 |
| `FixedAmount` | amount off the subtotal | greater than 0 and not more than the subtotal |

## API endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/orders?search=&customerId=&sortBy=Date\|Total&sortDescending=&page=&pageSize=` | Filtered, sorted, paged list |
| `GET` | `/api/orders/{id}` | Order details |
| `POST` | `/api/orders` | Create an order with its items |
| `PUT` | `/api/orders/{id}` | Replace the items and discount |
| `DELETE` | `/api/orders/{id}` | Soft delete |
| `GET` | `/api/orders/discount-preview?discountType=&discountValue=&subTotal=` | Calculate a discount without saving |
| `GET` | `/api/customers` | Customers lookup (seeded) |

Errors are returned as `ProblemDetails`: `400` for validation and business rules, `404` for missing resources. Ready-made requests are in `src/OrderManagement.Api/OrderManagement.Api.http`.

## Front end

| Requirement | Implementation |
|---|---|
| Orders list with filtering | `Pages/Orders/Index`: search, customer, sort and paging, all served by the specification-based API |
| Debounced search and partial updates | `orders-index.js` waits for typing to pause, then replaces only the table with the `_OrdersTable` partial |
| Create order with dynamic item rows | `Pages/Orders/Create` + `order-form.js`, using a `<template>` row that can be added and removed |
| Edit order | `Pages/Orders/Edit` reuses the same form (`_OrderFormFields`), base page model (`OrderFormModel`) and script, pre-filled with the order. The customer is read-only. |
| Dynamic discount strategies | Changing the discount or any item calls the API preview and updates the summary |
| Delete with confirmation | Bootstrap modal, then a `fetch` POST that refreshes only the table |
| Fetch API, ES6 modules, no jQuery | `wwwroot/js/modules` (`http`, `debounce`, `validation`) and `wwwroot/js/pages` |

The browser only calls Razor Page handlers (same origin). The handlers call the API through a typed `HttpClient` (`Services/OrdersApiClient.cs`), so no CORS is needed.

## Getting started

### Prerequisites

- .NET 10 SDK
- SQL Server LocalDB, which is installed with Visual Studio. To use another server, change `ConnectionStrings:DefaultConnection` in `src/OrderManagement.Api/appsettings.json`.
- Visual Studio 2026, or VS Code with C# Dev Kit. Visual Studio 2022 cannot build .NET 10 projects.

### Run

```bash
# once: trust the HTTPS development certificate
dotnet dev-certs https --trust

# terminal 1: API (https://localhost:7001/swagger)
dotnet run --project src/OrderManagement.Api

# terminal 2: Web UI (https://localhost:7002)
dotnet run --project src/OrderManagement.Web
```

In Development, the API creates the database, applies the migrations and seeds three customers on startup.

In Visual Studio, set **Multiple startup projects** (`OrderManagement.Api` and `OrderManagement.Web`) and press F5.

The Web app finds the API through `OrdersApi:BaseUrl` in `src/OrderManagement.Web/appsettings.json`.
