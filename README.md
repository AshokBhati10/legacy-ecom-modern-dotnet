# LegacyEcom Modern (.NET 8)

A modernization of the legacy ASP.NET MVC 5 / .NET Framework 4.7 e-commerce
application (`Ecommerce.*`) into **.NET 8** using **ASP.NET Core Minimal APIs**,
**EF Core**, **SQL Server**, and **ASP.NET Core Identity**.

The original repository is the untouched source of truth for behavior; this
repository contains only the new implementation, with a clean history.

## What was modernized

| Legacy | Modern |
|---|---|
| ASP.NET MVC 5 controllers + Razor views | Minimal API endpoints (`MapGet`/`MapPost`/`MapPut`/`MapDelete`) |
| Entity Framework 6 (EDMX, Database First) | EF Core 8, code-first mappings, migrations |
| Unity container | Built-in ASP.NET Core DI |
| OWIN + ASP.NET Identity 2 | ASP.NET Core Identity + cookie authentication |
| `web.config` connection strings | `appsettings.json` (+ environment overrides) |
| `Session`-based cart + checkout wizard | Server session cart + checkout state, same flow |

All business rules are preserved: sale-price-wins pricing, variant price
adjustments, 99-unit max per cart line, $4.99 standard shipping (free at $50+),
$12.99 express shipping, 8% tax, the Address → Shipping → Payment →
Confirmation checkout wizard, Card/CashOnDelivery payment, `LE-YYYYMMDD-XXXXXX`
order numbers, snapshot order lines, anonymous session carts merged into
persistent carts on login, and the legacy password/lockout policy.

## Solution layout

```
src/
  LegacyEcom.Domain/          Entities, domain interfaces (no dependencies)
  LegacyEcom.Application/     DTOs, services (workflows), interfaces
  LegacyEcom.Infrastructure/  EF Core DbContext, configurations, repositories,
                              Identity stores, migrations, seed data
  LegacyEcom.Api/             Minimal API endpoints, auth/session/antiforgery
                              pipeline, validation + ProblemDetails middleware
tests/
  LegacyEcom.Tests/           xUnit: pricing, services, API integration tests
docs/
  Architecture.md  Migration.md  API.md  Database.md  Running-Locally.md
  Knowledge-Bytes.md
```

## Quick start

```bash
# 1. Configure the connection (never commit real secrets)
export ConnectionStrings__EcommerceDb="Server=localhost;Database=LegacyEcommerceModernDb;User Id=sa;Password=<pwd>;TrustServerCertificate=True;Encrypt=True"

# 2. Build + apply migrations + seed + run
dotnet build LegacyEcom.Modern.sln
cd src/LegacyEcom.Api
dotnet ef database update --project ../LegacyEcom.Infrastructure
ASPNETCORE_ENVIRONMENT=Development dotnet run   # seeds the catalog in Development
```

Swagger UI (Development): `http://localhost:5099/swagger`

## API at a glance

- `GET /api/products`, `GET /api/products/{id}`, `GET /api/products/featured`
- `GET /api/categories`, `GET /api/categories/tree`
- `GET/POST/PUT/DELETE /api/cart...` (session cart; persisted per user when logged in)
- `GET/POST /api/checkout/address|shipping|summary`, `POST /api/checkout/place-order`
- `GET /api/orders`, `GET /api/orders/{id}` (auth required)
- `POST /api/auth/register|login|logout`, `GET /api/auth/me`, `GET /api/auth/xsrf-token`
- `GET /api/health`

State-changing calls require the antiforgery pair: `GET /api/auth/xsrf-token`
sets the readable `XSRF-TOKEN` cookie; send it back in the `X-XSRF-TOKEN`
header. Fetch a fresh token after login/logout/register (tokens bind to the
authenticated identity).

## Frontend (legacy-stack storefront)

`frontend/` is a static HTML storefront built with the **original legacy
frontend technology stack** — jQuery 3.4.1, jQuery UI 1.12.1, Bootstrap 3,
jQuery Validate + Unobtrusive Validation, DataTables, and Fancybox 3 — talking
to the .NET 8 Minimal API as JSON. **The backend is the completed .NET 8
modernization and is not being reverted to the legacy MVC architecture; no
backend code was changed for this frontend.**

```bash
# 1. Start the backend
cd src/LegacyEcom.Api
ASPNETCORE_ENVIRONMENT=Development dotnet run   # http://localhost:5174

# 2. Start the storefront (from frontend/; Node built-ins only, no npm install)
cd frontend
node dev-server.mjs        # http://localhost:5173 — /api is proxied to the backend
```

The dev server proxies `/api` to the backend, so cookie auth + session cart +
XSRF work same-origin with no backend CORS change. See `frontend/README.md`
for the stack rationale, architecture, and the legacy MVC → API-based
differences.

Pages: Home, Products (search / category tree / pagination), Product detail
(Fancybox gallery, variants), Cart + mini-cart, 4-step Checkout wizard
(Address → Shipping → Payment → Confirmation), Login, Register, Order history
(DataTables) + detail. Playwright E2E: `npx playwright test frontend/e2e/journey.spec.ts`
(3 specs, full journey against the real backend).

## Verification

- `dotnet build` — 0 warnings, 0 errors.
- **39 automated xUnit tests pass**: 11 pricing, 16 service, 12 API integration
  (full register → cart → checkout → order journey, auth rejections, XSRF
  enforcement). Note: in this sandbox `dotnet test` cannot run because the
  runtime redirects `TcpClient` connections to the egress proxy, breaking the
  vstest↔testhost channel; tests were executed with an in-process xUnit runner
  instead. On a normal machine `dotnet test` works as-is.
- **45/45 end-to-end checks pass** against real SQL Server 2022 Express,
  covering catalog, auth, cart, the full checkout wizard (Card + COD), order
  persistence, and cross-session order isolation.

See `docs/` for architecture, the legacy→modern migration mapping, the full
API reference, database notes, and the local run guide.
