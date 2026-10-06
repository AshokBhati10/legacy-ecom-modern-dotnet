# 05 — Authentication and Authorization

### Byte 16: Cookie authentication with ASP.NET Core Identity

**Builds on:** Byte 7

**In plain terms:**

Login state is a signed, HTTP-only cookie issued by ASP.NET Core Identity. `UseAuthentication()` reads the cookie into `http.User` on every request; `UseAuthorization()` enforces `.RequireAuthorization()` on protected endpoints. The password policy, unique-email rule, and 5-failures → 5-minute lockout are configured in one place.

**The code:**

```csharp
// ServiceExtensions.cs — Identity options (one place for all auth policy).
services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.User.RequireUniqueEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<EcommerceDbContext>()
    .AddDefaultTokenProviders();

// The auth cookie: 25-minute sliding expiration, API-style 401/403.
services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(25);
    options.SlidingExpiration = true;
    options.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    // ... same for OnRedirectToAccessDenied → 403
});
```

Note the redirect overrides: browser apps get sent to a login page, but this is an API, so unauthenticated requests get a clean `401` instead. `AppUser` (in `Infrastructure/Identity`) is the Identity user entity stored via the same `EcommerceDbContext`.

### Byte 17: The XSRF token dance

**Builds on:** Byte 16

**In plain terms:**

Cookies are sent automatically by browsers — including by malicious sites — so every state-changing request (POST/PUT/DELETE) must also carry a secret the attacker's site can't know. The client fetches it from `GET /api/auth/xsrf-token`, which sets a readable `XSRF-TOKEN` cookie; the client echoes that value in the `X-XSRF-TOKEN` header. The token is bound to the current identity, so it must be refreshed after login, logout, or register.

**The code:**

```csharp
// AuthEndpoints.cs — issuing the token.
group.MapGet("/xsrf-token", (HttpContext http, IAntiforgery antiforgery) =>
    {
        var tokens = antiforgery.GetAndStoreTokens(http);
        http.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
            new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.Lax });
        return Results.NoContent();
    });
```

```csharp
// Filters/ValidationFilter.cs — enforcing it (opt-in per endpoint).
public static TBuilder RequireAntiforgeryToken<TBuilder>(this TBuilder builder)
{
    builder.AddEndpointFilter(async (context, next) =>
    {
        var antiforgery = context.HttpContext.RequestServices
            .GetRequiredService<IAntiforgery>();
        try { await antiforgery.ValidateRequestAsync(context.HttpContext); }
        catch (AntiforgeryValidationException)
        {
            return Results.Problem(title: "Invalid anti-forgery token.",
                detail: "Send the XSRF-TOKEN cookie value in the X-XSRF-TOKEN header. ...",
                statusCode: StatusCodes.Status400BadRequest);
        }
        return await next(context);
    });
    return builder;
}
```

Note there are *two* cookies involved: the framework's own HTTP-only antiforgery cookie (default name, unreadable by JavaScript) and the readable `XSRF-TOKEN` copy. Validation compares the header value against the framework cookie. A missing or stale token is a `400`, not a `401` — it's a malformed request, not an identity problem.

### Byte 18: Authorization and order ownership

**Builds on:** Bytes 16–17

**In plain terms:**

Two authorization patterns exist. Order history requires a signed-in user (`.RequireAuthorization()` → 401 otherwise). Order *details* are subtler: a guest who just checked out must see their confirmation without an account, but nobody else may. So `CanViewOrder` allows the order's owner *or* the session that just placed it — and everyone else gets a `404`, deliberately hiding whether the order exists.

**The code:**

```csharp
// OrderEndpoints.cs
group.MapGet("/", (HttpContext http, IAccountService account) =>
    {
        var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Results.Ok(account.GetOrderHistory(userId));
    })
    .RequireAuthorization()   // 401 if anonymous
    .Produces(401);

group.MapGet("/{id:int}", (int id, HttpContext http, ICheckoutService checkout) =>
    {
        var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var lastOrderId = http.Session.GetLastOrderId();

        // Do not leak order existence: non-owners get 404.
        if (!checkout.CanViewOrder(id, userId, lastOrderId))
            return Results.NotFound(new { message = $"Order {id} not found." });
        // ...
    });
```

```csharp
// CheckoutService.cs
public bool CanViewOrder(int orderId, string? userId, int? sessionLastOrderId)
{
    if (sessionLastOrderId == orderId) return true;   // guest confirmation flow
    if (string.IsNullOrEmpty(userId)) return false;
    var order = _orders.GetById(orderId);
    return order is not null &&
           string.Equals(order.UserId, userId, StringComparison.Ordinal);
}
```

The 404-instead-of-403 is a security decision, not an accident: returning 403 would confirm "this order exists but isn't yours." When debugging "I can't see my order," check identity first (cookie present? not expired after 25 idle minutes?), then session state.

## PUTTING IT TOGETHER

Identity issues the cookie, the pipeline turns it into `http.User`, and endpoints declare their requirements: antiforgery on mutations, `RequireAuthorization` for account data, and a custom ownership check for order details that hides order existence from strangers. Auth policy (passwords, lockout, cookie lifetime) lives in exactly one place — `ServiceExtensions` — so there's never a question of which rule is in force.
