# 01 — System Architecture

### Byte 1: What this application does

**Builds on:** None — starting point

**In plain terms:**

This is the backend of an online store. It serves a product catalog, manages shopping carts, runs a checkout wizard (address → shipping → payment), creates orders, and handles customer accounts. Everything is exposed as JSON API endpoints — there are no web pages in this project.

**The code:**

```csharp
// Program.cs — the entire HTTP surface is registered here:
app.MapCatalogEndpoints();    // /api/products, /api/categories
app.MapCartEndpoints();       // /api/cart
app.MapCheckoutEndpoints();   // /api/checkout
app.MapOrderEndpoints();      // /api/orders
app.MapAuthEndpoints();       // /api/auth
```

Each `Map*` call registers a group of related endpoints. If you want to know what the system *can do*, this list is the table of contents.

### Byte 2: The four layers and the dependency rule

**Builds on:** Byte 1

**In plain terms:**

The code is split into four projects arranged in layers. The strict rule is that inner layers never know about outer layers — the business logic has no idea it's running inside a web server, which is what makes it testable without HTTP.

**The code:**

```
LegacyEcom.Api            → knows about everything (wires it up)
LegacyEcom.Application    → knows only Domain (services, DTOs, pricing)
LegacyEcom.Infrastructure → implements Domain interfaces (EF Core, SQL)
LegacyEcom.Domain         → knows about nothing (entities, interfaces)
```

The direction of the arrows matters: `Application` calls `IProductRepository`, an interface defined in `Domain`. It never mentions Entity Framework. `Infrastructure` provides the real implementation. This is why the 16 service tests can run against an in-memory fake without a database.

### Byte 3: The golden path — request to database and back

**Builds on:** Byte 2

**In plain terms:**

Every request travels the same road: an endpoint receives it, a service decides what it means, a repository fetches or saves data, EF Core translates that into SQL. On the way back, entities are converted into DTOs so database details never leak into API responses.

**The code:**

```csharp
// CatalogEndpoints.cs — the endpoint is thin: bind, delegate, return.
group.MapGet("/{id:int}", (int id, ICatalogService catalog) =>
{
    var detail = catalog.GetDetail(id);
    return detail is null
        ? Results.NotFound(new { message = $"Product {id} not found." })
        : Results.Ok(detail);
});
```

```csharp
// CatalogService.cs — the service does the orchestration.
public ProductDetailDto? GetDetail(int productId)
{
    var p = _products.GetById(productId);   // repository, not EF Core
    if (p is null) return null;
    return new ProductDetailDto( ... );      // entity → DTO conversion
}
```

Notice what the endpoint does *not* do: no SQL, no business rules, no mapping logic beyond choosing the status code. If you keep this separation in mind, you will always know which layer a bug lives in.

## PUTTING IT TOGETHER

The application is a JSON API organized in four layers with dependencies pointing strictly inward. Endpoints are thin translators between HTTP and services; services contain the business logic and talk only to repository interfaces; repositories are the only classes that touch EF Core. Data always returns to the client as DTOs, never as database entities. When you read the remaining files, watch how each new concept slots into this single request path.
