# Architecture Overview — LegacyEcom Modern

A one-page map of the codebase. Read this first, then follow the numbered files.

## What the system is

A JSON API backend for an e-commerce store, built with **ASP.NET Core 8 Minimal APIs** and **Entity Framework Core 8** against **SQL Server**. There is no UI in this repository — it exposes REST-style endpoints (`/api/products`, `/api/cart`, `/api/checkout`, `/api/orders`, `/api/auth`) that a storefront would call.

## The four projects (outer → inner)

```
LegacyEcom.Api            HTTP layer: endpoints, middleware, session, auth wiring
    └── LegacyEcom.Application    Business logic: services, DTOs, pricing rules
            └── LegacyEcom.Domain    Entities + interfaces only (no dependencies)
LegacyEcom.Infrastructure  EF Core: DbContext, mappings, repositories, Identity user
```

**Dependency rule:** nothing inner knows about anything outer. `Domain` has zero external dependencies; `Application` depends only on `Domain`; `Infrastructure` implements `Domain`'s interfaces; `Api` wires everything together.

## The golden path of a request

```
HTTP request
  → Minimal API endpoint (binds input, thin)
  → Application service (business logic, orchestration)
  → Repository interface (data access contract)
  → EF Core DbContext (LINQ → SQL)
  → SQL Server
...and back up as DTOs, never entities.
```

**Key vocabulary used across these bytes:**

- **Endpoint** — a single Minimal API route handler (replaces MVC controller actions).
- **DTO** (Data Transfer Object) — the shape of data crossing the API boundary; separate from database entities.
- **Service** — a class holding business logic and workflows (e.g. `CartService`).
- **Repository** — the only classes allowed to touch EF Core directly (e.g. `ProductRepository`).
- **DbContext** (`EcommerceDbContext`) — EF Core's gateway to the database.
- **Session** — per-visitor server-side storage, used for the guest cart and checkout wizard state.

## Where to look when something breaks

| Symptom | Look in |
|---|---|
| Wrong HTTP status / response shape | `src/LegacyEcom.Api/Endpoints/*` |
| Request rejected before reaching logic | `Filters/ValidationFilter.cs`, antiforgery in `Program.cs` pipeline |
| Wrong totals / business rule violated | `Application/Services/*`, `Application/Pricing/PriceCalculator.cs` |
| Wrong data / missing rows | `Infrastructure/Repositories/Repositories.cs`, then `Data/Configurations/*` |
| Login/session/cookie problems | `Api/Extensions/ServiceExtensions.cs` (Identity + cookie options) |
| Startup / migration failures | `Api/Program.cs` (top: `MigrateAsync` / `EnsureCreatedAsync` block) |
| 500 with no useful message | `Api/Middleware/ExceptionHandlingMiddleware.cs` (check server logs) |

Files `01`–`07` walk each of these areas as short, self-contained Knowledge Bytes.
