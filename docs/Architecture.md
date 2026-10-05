# Architecture

## Layering

```
┌─────────────────────────────────────────────────────────┐
│  LegacyEcom.Api                                         │
│  Minimal API endpoint classes (thin: bind → validate →  │
│  call service → map to DTO)                             │
│  Pipeline: exception middleware → validation filter →   │
│  session → auth → antiforgery → endpoints               │
├─────────────────────────────────────────────────────────┤
│  LegacyEcom.Application                                 │
│  Services: CatalogService, CartService, CheckoutService, │
│  AccountService, PriceCalculator                         │
│  DTOs: request/response models, separate from entities  │
├─────────────────────────────────────────────────────────┤
│  LegacyEcom.Infrastructure                              │
│  EcommerceDbContext (IdentityDbContext), EF Core         │
│  configurations, repositories, UnitOfWork, DbSeeder     │
├─────────────────────────────────────────────────────────┤
│  LegacyEcom.Domain                                      │
│  Entities (Product, Order, CartItem, …), repository and │
│  service interfaces. No external dependencies.          │
└─────────────────────────────────────────────────────────┘
```

Dependencies point inward: Api → Application → Domain; Infrastructure →
Domain + Application interfaces. Domain knows nothing of EF Core.

## Request flow (example: add to cart)

1. `POST /api/cart/items` hits `CartEndpoints` — binds `AddCartItemRequest`,
   data-annotation validation runs via the validation filter (400 ProblemDetails
   on failure).
2. Antiforgery runs for the route (valid `X-XSRF-TOKEN` header required).
3. `CartService.AddItemAsync` loads the product (+variant) through
   `IProductRepository`, computes the effective unit price
   (sale price wins; variant adjustment added), and enforces the 99-unit cap.
4. Anonymous users: cart lines are kept in server session. Authenticated users:
   lines persist to `CartItems` via `ICartRepository`.
5. The endpoint returns the updated `CartDto` (lines, subtotal, item count).

## Key decisions

- **Thin endpoints, fat services.** Endpoints do HTTP binding/validation/status
  codes only. All workflows (cart math, checkout wizard state, order creation)
  live in Application services, which are unit-testable without HTTP.
- **Repositories over DbContext in services.** Services depend on repository
  interfaces; EF Core stays inside Infrastructure. The unit of work scopes one
  `DbContext` per request.
- **DTOs ≠ entities.** API shapes (`ProductDetailDto`, `OrderDto`, …) are
  separate from EF entities, so persistence changes never leak into the API.
- **Session cart, persistent cart.** Anonymous carts live in server-side
  session (legacy behavior). On login, the session cart merges into the
  database-backed cart (sums quantities, caps at 99) — the legacy merge rule.
- **Checkout wizard state in session.** Address → shipping method are stored
  server-side between steps; the API returns 409 if steps are attempted out of
  order. Placing the order snapshots product data into `OrderLines`, clears the
  cart, and stores the order id in session so guests can view their own order.
- **Cookie auth, API-style.** ASP.NET Core Identity with cookie authentication;
  401/403 are returned as ProblemDetails instead of login redirects, since the
  clients are API consumers, not browsers loading Razor pages.
- **Antiforgery for cookie-authenticated mutations.** Cookie auth is
  vulnerable to CSRF, so state-changing endpoints require the
  `XSRF-TOKEN` cookie / `X-XSRF-TOKEN` header pair.

## Cross-cutting concerns

- **Errors:** `ExceptionHandlingMiddleware` converts unhandled exceptions to
  RFC 7807 ProblemDetails; domain failures (not found, conflict, validation)
  map to 404/409/400 explicitly at the endpoint layer.
- **Validation:** data annotations on DTOs (+ `IValidatableObject` for
  card-payment fields); a single endpoint filter produces 400 ProblemDetails.
- **AuthN/Z:** Identity cookie auth; `[Authorize]`-equivalent
  `RequireAuthorization()` on order history/detail and checkout completion.
- **Config:** `appsettings.json` + environment-specific files; connection
  strings via `ConnectionStrings__EcommerceDb` env var (no secrets in files).
- **Docs:** Swagger/OpenAPI in Development.
