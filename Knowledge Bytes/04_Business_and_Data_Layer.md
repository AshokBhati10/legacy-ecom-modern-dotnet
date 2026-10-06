# 04 — Business and Data Layer

### Byte 11: Services hold the business logic; endpoints stay thin

**Builds on:** Byte 3

**In plain terms:**

All rules about money and carts live in `Application/Services`, not in endpoints. `PriceCalculator` is pure math with no I/O; services like `CartService` orchestrate repositories and enforce rules (e.g. max 99 units per line). Endpoints never compute anything — they just call a service and pick a status code.

**The code:**

```csharp
// PriceCalculator.cs — pure functions, no database, no HTTP.
public static decimal CalculateShippingCost(decimal subTotal, string? shippingMethod)
{
    if (string.Equals(shippingMethod, ExpressShippingCode, StringComparison.OrdinalIgnoreCase))
        return ExpressShippingFlatRate;          // $12.99 express

    // Standard: free once the threshold is reached.
    return subTotal >= FreeShippingThreshold ? 0m : StandardShippingFlatRate;  // $4.99 / free $50+
}
```

```csharp
// CartService.cs — the 99-unit rule lives here, enforced on every mutation.
private const int MaxQuantityPerLine = 99;
// ...
list.Add(existing with { Quantity = Math.Min(existing.Quantity + quantity, MaxQuantityPerLine) });
```

Because pricing is pure functions, the 11 pricing tests verify the money rules without a database, a web server, or HTTP. If a rule changes, there is exactly one place to change it.

### Byte 12: ServiceResult — business failures are values, not exceptions

**Builds on:** Byte 11

**In plain terms:**

Services never throw when a business rule fails (unknown product, empty cart). They return `ServiceResult<T>`: either `Ok(data)` or `Fail(message)`. The endpoint then maps failure to the right 4xx status. This keeps control flow explicit and makes "what can go wrong" visible in the method signature.

**The code:**

```csharp
// Application/Interfaces/Services.cs
public sealed class ServiceResult<T>
{
    public bool Success { get; }
    public string? Message { get; }
    public T? Data { get; }
    // ...
}
```

```csharp
// CartService.cs — "product not found" is a value, not an exception.
var product = _products.GetById(productId);
if (product is null)
    return ServiceResult<IReadOnlyList<SessionCartItem>>.Fail("Product not found.");
```

```csharp
// CartEndpoints.cs — the endpoint translates the value to HTTP.
var result = cart.AddItem(...);
if (!result.Success)
    return Results.BadRequest(new { message = result.Message });
```

Rule of thumb for this codebase: if the *user* did something wrong, it's a `ServiceResult.Fail`; if the *system* broke, it's an exception caught by the middleware from Byte 9.

### Byte 13: Entities vs DTOs — the boundary

**Builds on:** Byte 11

**In plain terms:**

Database entities (`Domain/Entities`) and API shapes (`Application/DTOs`) are different classes on purpose. Entities carry persistence concerns and navigation properties; DTOs carry exactly what the client needs — nothing more. Services convert between them, so a database change never accidentally leaks into the API.

**The code:**

```csharp
// Domain/Entities/Product.cs — persistence-ignorant POCO, never leaves the server.
public class Product
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

    public decimal EffectivePrice =>
        SalePrice.HasValue && SalePrice.Value > 0 ? SalePrice.Value : Price;
}
```

```csharp
// Application/DTOs/Catalog — the client sees this instead (excerpt).
public record ProductCardDto(int Id, string Name, string Slug,
    decimal Price, decimal? SalePrice, decimal EffectivePrice, string? ThumbnailUrl);
```

Notice `EffectivePrice` (sale price wins) is computed once on the entity and then carried into DTOs — the rule is defined in one place and every response agrees. If you add a column to an entity, no API response changes until you deliberately add it to a DTO.

### Byte 14: Repositories are the only classes that touch EF Core

**Builds on:** Byte 13

**In plain terms:**

Services never write LINQ against the `DbContext` directly. All database access goes through repository interfaces defined in `Domain` (e.g. `IProductRepository`) and implemented once in `Infrastructure/Repositories/Repositories.cs`. The `IUnitOfWork` (a thin wrapper over `SaveChanges`) lets a service persist several changes atomically.

**The code:**

```csharp
// Domain/Interfaces/Repositories.cs — the contract; no EF Core in sight.
public interface IProductRepository
{
    Product? GetById(int id);
    (IList<Product> Items, int TotalCount) GetListing(int? categoryId, string? q, int page, int pageSize);
    IList<Product> GetFeatured(int take);
    IList<Product> GetRelated(int productId, int take);
}
```

```csharp
// Infrastructure/Repositories/Repositories.cs — the single implementation.
// Include() is used so listing/detail never N+1.
public Product? GetById(int id) =>
    _db.Products
        .Include(x => x.Category)
        .Include(x => x.Images)
        .Include(x => x.Variants)
        .FirstOrDefault(x => x.Id == id && x.IsActive);
```

```csharp
// One atomic save across repositories (CheckoutService.PlaceOrder):
_orders.Add(order);
_unitOfWork.SaveChanges();
```

If a query is slow or wrong, `Repositories.cs` is the file. If you need a new query, add it to the interface first — that keeps services honest about what data access they need.

### Byte 15: Dependency injection — everything is scoped

**Builds on:** Byte 14

**In plain terms:**

`Api/Extensions/ServiceExtensions.cs` wires every interface to its implementation, and nearly everything is registered **scoped** — one instance per HTTP request. That matches the `DbContext` lifetime, so a whole request (endpoint → services → repositories) shares one database connection and one unit of work.

**The code:**

```csharp
// ServiceExtensions.cs
services.AddDbContext<EcommerceDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("EcommerceDb")));

services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<IOrderRepository, OrderRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();

services.AddScoped<ICatalogService, CatalogService>();
services.AddScoped<ICartService, CartService>();
services.AddScoped<ICheckoutService, CheckoutService>();
```

Two consequences worth remembering: never store per-request state in a service field (a service instance is reused only within its request, but fields invite mistakes — `PriceCalculator` is static and stateless for exactly this reason), and never resolve a scoped service from a singleton. The integration tests exploit this same container by swapping the `DbContext` registration for an in-memory one.

## PUTTING IT TOGETHER

Business logic lives in services and pure functions, never in endpoints; business failures travel as `ServiceResult` values while system failures become exceptions caught at the edge. Entities and DTOs are separate classes with services converting between them, repositories are the sole gateway to EF Core, and DI wires it all as scoped-per-request so one request equals one unit of work. This is why the service tests can verify cart and checkout rules with no web server at all.
