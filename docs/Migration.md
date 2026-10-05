# Migration: legacy → modern

Source of behavior: the legacy `Ecommerce.*` solution (MVC 5, .NET Framework
4.7, EF 6 EDMX). The original repository is **not modified**; everything below
was re-implemented in this repository.

## Framework mapping

| Concern | Legacy | Modern |
|---|---|---|
| Web framework | ASP.NET MVC 5, `.NET Framework 4.7` | ASP.NET Core 8, Minimal APIs |
| Controllers → endpoints | `ProductController`, `CartController`, `CheckoutController`, `AccountController`, `OrderController` | `CatalogEndpoints`, `CartEndpoints`, `CheckoutEndpoints`, `AuthEndpoints`, `OrderEndpoints` (`MapGet`/`MapPost`/`MapPut`/`MapDelete`) |
| Data access | EF 6 Database-First (EDMX), `EcommerceDbContext` | EF Core 8 code-first, explicit `IEntityTypeConfiguration<T>` per entity, migrations |
| DI | Unity container | Built-in `IServiceCollection` |
| Auth | OWIN + ASP.NET Identity 2, 25-min cookie | ASP.NET Core Identity, cookie auth, API-style 401/403 |
| Config | `web.config` | `appsettings.json` + env overrides |
| Session | In-process session cart | Distributed-cache-ready server session cart |
| Anti-forgery | MVC `@Html.AntiForgeryToken()` | `XSRF-TOKEN` cookie + `X-XSRF-TOKEN` header |
| Views | 19 Razor views | Out of scope by design: this is a backend/API modernization; the legacy Razor UI remains reference-only and no SPA framework was introduced |

## Data model mapping

Table/column names, keys, relationships, lengths, and nullability from the
legacy schema are preserved by the EF Core configurations (see `docs/Database.md`).
New: ASP.NET Core Identity tables (`AspNetUsers`, `AspNetRoles`, …) replace the
Identity 2 tables; the app's own `Customers`/`Addresses` tables keep their
legacy shape and link to users by email.

## Business rules preserved (verified by tests)

- Effective price: `SalePrice` wins when positive, else `Price`.
- Variant adjustment is added to the effective unit price.
- Cart line quantity capped at 99.
- Standard shipping $4.99, free when subtotal ≥ $50; express $12.99; tax 8%.
- Checkout wizard order enforced server-side (Address → Shipping → Payment).
- Payment methods: Card (requires card fields) or Cash on Delivery.
- Order numbers: `LE-YYYYMMDD-XXXXXX`.
- Order lines snapshot product name/SKU/unit price at purchase time.
- Anonymous cart in session; authenticated cart in `CartItems`; merge on login.
- Password policy: ≥6 chars, upper + lower + digit + non-alphanumeric, unique
  email; 5 failed logins → 5-minute lockout.
- Search is case-insensitive (legacy SQL Server CI collation semantics, now
  explicit in the query so it holds on every provider).

## Behavior changes (deliberate)

- Endpoints return JSON + proper status codes instead of Razor views/redirects.
- 401/403 are ProblemDetails, not redirects to `/Account/Login`.
- Antiforgery uses a header instead of a hidden form field.
- `GET /api/auth/xsrf-token` must be called after login/logout/register —
  tokens are bound to the authenticated identity.

## Known limitations

- The EF Core migration creates a **fresh** database. An in-place upgrade of a
  populated legacy database (data migration from EF6/Identity 2 tables) has not
  been performed or verified.
- Legacy Identity 2 password hashes are not imported; users re-register.
- No UI is shipped in this repository (backend modernization only).
