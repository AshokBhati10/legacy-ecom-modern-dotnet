# 03 — Minimal API Request Flow

### Byte 6: Endpoint groups instead of controllers

**Builds on:** Byte 3

**In plain terms:**

There are no controllers here. Each feature area is a static class with a `Map*` method that registers its routes on a URL group (e.g. everything under `/api/cart`). A route is just a lambda that takes its inputs as parameters — ASP.NET Core injects services like `ICartService` automatically.

**The code:**

```csharp
// CartEndpoints.cs
public static void MapCartEndpoints(this IEndpointRouteBuilder app)
{
    var group = app.MapGroup("/api/cart").WithTags("Cart");

    group.MapGet("/", (HttpContext http, ICartService cart) =>
            Results.Ok(cart.GetCart(http.Session.GetCartItems())))
        .WithName("GetCart")
        .Produces<CartDto>();

    group.MapPost("/items", (HttpContext http, AddCartItemRequest request, ICartService cart) => { ... })
        .AddEndpointFilter<ValidationFilter>()
        .RequireAntiforgeryToken()
        .Produces<CartDto>()
        .Produces(400);
}
```

The modifiers chained after the lambda are the interesting part: `.AddEndpointFilter<ValidationFilter>()` runs request validation, `.RequireAntiforgeryToken()` enforces the XSRF check, and `.Produces<>()` documents the response for Swagger. Routing, validation, and security are declared right next to the handler instead of scattered across attributes and base classes.

### Byte 7: The pipeline — middleware order in Program.cs

**Builds on:** Byte 6

**In plain terms:**

Before any endpoint runs, the request passes through middleware in the exact order listed in `Program.cs`. Order matters: the exception handler is first so it can catch failures from everything below it, and session/auth run before endpoints so handlers can use them.

**The code:**

```csharp
// Program.cs
app.UseMiddleware<ExceptionHandlingMiddleware>();  // catches everything below

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(...);
}

app.UseSession();         // makes http.Session available
app.UseAuthentication();  // fills http.User from the auth cookie
app.UseAuthorization();   // enforces RequireAuthorization()

app.MapCatalogEndpoints();
...
```

If you ever wonder "why is `http.Session` empty?" or "why is the user anonymous here?" — the answer is almost always in this ordering. Anything that needs session or identity must be registered after these lines.

### Byte 8: Validation without MVC

**Builds on:** Byte 7

**In plain terms:**

Minimal APIs in .NET 8 don't validate request models automatically the way MVC did, so this project adds its own endpoint filter. Any DTO in the `LegacyEcom.Application.DTOs` namespace gets its data-annotation attributes checked, and failures become a 400 validation problem before the handler ever runs.

**The code:**

```csharp
// ValidationFilter.cs
var results = new List<ValidationResult>();
Validator.TryValidateObject(argument, validationContext, results, validateAllProperties: true);
// ...
if (errors.Count > 0)
    return Results.ValidationProblem(errors);

return await next(context);
```

```csharp
// CartDtos.cs — what gets validated (note the property: target on records)
public record AddCartItemRequest(
    [property: Range(1, int.MaxValue)] int ProductId,
    int? VariantId,
    [property: Range(1, 99)] int Quantity = 1);
```

Two things worth knowing: validation only applies to endpoints that opt in via `.AddEndpointFilter<ValidationFilter>()`, and on positional records the attributes need the `property:` prefix or the validator silently ignores them — a real gotcha this codebase already learned.

### Byte 9: Errors become ProblemDetails

**Builds on:** Byte 7

**In plain terms:**

Unhandled exceptions never leak stack traces to clients. The first middleware in the pipeline catches everything, logs the full exception server-side, and returns an RFC 7807 problem response. Business failures (bad cart item, missing product) never throw at all — they return `ServiceResult.Fail`, and the endpoint maps that to a 400/404.

**The code:**

```csharp
// ExceptionHandlingMiddleware.cs
catch (Exception ex)
{
    _logger.LogError(ex, "Unhandled exception processing {Method} {Path}.", ...);
    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    // ...
    var problem = new ProblemDetails
    {
        Status = 500,
        Title = "An unexpected error occurred.",
        Detail = _env.IsDevelopment() ? ex.Message : "Please try again later.",
        Instance = context.Request.Path
    };
    await context.Response.WriteAsJsonAsync(problem);
}
```

The design split is deliberate: *expected* failures are return values (`ServiceResult`), *unexpected* failures are exceptions caught once at the edge. If you see a 500, the details are in the server logs, not the response.

### Byte 10: A full request walkthrough — product listing

**Builds on:** Bytes 6–9

**In plain terms:**

Trace one real request end to end: `GET /api/products?q=headphones&page=1`. The endpoint binds query parameters, the catalog service orchestrates, the repository builds one LINQ query, EF Core turns it into SQL, and the result comes back as DTOs.

**The code:**

```csharp
// 1. Endpoint binds query string → parameters (CatalogEndpoints.cs)
group.MapGet("/", (ICatalogService catalog,
        int? categoryId = null, string? q = null,
        int page = 1, int pageSize = 12) =>
    Results.Ok(catalog.GetListing(categoryId, q, page, pageSize)));

// 2. Service orchestrates + maps entities → DTOs (CatalogService.cs)
var (items, totalCount) = _products.GetListing(categoryId, q, page, pageSize);
return new ProductListDto(items.Select(ToCard).ToList(), ...);

// 3. Repository builds a single composable query (Repositories.cs)
var query = _db.Products
    .Include(x => x.Images)
    .Where(x => x.IsActive);
if (!string.IsNullOrWhiteSpace(q))
{
    var term = q.Trim().ToLower();
    query = query.Where(x => x.Name.ToLower().Contains(term)
                          || x.Sku.ToLower().Contains(term));
}
var items = query.OrderBy(x => x.Name)
                 .Skip((page - 1) * pageSize).Take(pageSize).ToList();
```

Note the search uses `ToLower()` on both sides — SQL Server's `LIKE` is case-insensitive by collation, but EF Core's in-memory test provider is not, so the code states the intent explicitly rather than relying on the database. This is the entire request lifecycle in miniature; cart, checkout, and auth all follow the same shape.

## PUTTING IT TOGETHER

A request enters through a Minimal API endpoint group, passes middleware (exception handling → session → auth) in `Program.cs` order, gets validated by an opt-in filter, and then flows endpoint → service → repository → EF Core → SQL Server, returning as DTOs. Errors split cleanly: business failures are `ServiceResult` values mapped to 4xx, unexpected failures are caught once at the edge and logged. The product-listing walkthrough shows every layer doing exactly one job.
