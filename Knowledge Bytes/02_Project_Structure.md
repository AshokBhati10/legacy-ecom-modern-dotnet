# 02 — Project Structure

### Byte 4: What's inside each project

**Builds on:** Byte 2

**In plain terms:**

Each of the four projects has a distinct job and a predictable folder layout. Once you learn the layout, you can find anything: HTTP concerns live in `Api`, workflows in `Application`, database code in `Infrastructure`, and the shared vocabulary in `Domain`.

**The code:**

```
src/LegacyEcom.Api/
  Endpoints/        CatalogEndpoints, CartEndpoints, CheckoutEndpoints,
                    OrderEndpoints, AuthEndpoints   (the HTTP surface)
  Extensions/       ServiceExtensions.cs            (DI + auth + session setup)
  Filters/          ValidationFilter.cs             (request validation)
  Infrastructure/   SessionCartExtensions.cs        (session helpers)
  Middleware/       ExceptionHandlingMiddleware.cs  (error handling)
  Program.cs                                    (composition root)

src/LegacyEcom.Application/
  DTOs/             Auth, Cart, Catalog, Checkout, Orders  (API shapes)
  Interfaces/       Services.cs                 (ICartService, ICheckoutService…)
  Pricing/          PriceCalculator.cs           (pure pricing math)
  Services/         CatalogService, CartService, CheckoutService, AccountService

src/LegacyEcom.Infrastructure/
  Data/             EcommerceDbContext.cs, Configurations/, Migrations/, DbSeeder.cs
  Identity/         AppUser.cs                  (Identity user entity)
  Repositories/     Repositories.cs             (all repository implementations)

src/LegacyEcom.Domain/
  Entities/         Product, Order, CartItem, … (plain C# classes)
  Interfaces/       Repositories.cs             (IProductRepository, IUnitOfWork…)

tests/LegacyEcom.Tests/
  Pricing/  Services/  Api/                     (mirrors the three test levels)
```

A useful habit: `Endpoints` ↔ `Services` ↔ `Repositories` have matching feature names (Cart, Checkout, Catalog), so you can trace a feature vertically through all three.

### Byte 5: Where to start when something breaks

**Builds on:** Byte 4

**In plain terms:**

Don't read the codebase top to bottom — start from the symptom and walk the golden path from Byte 3. Each layer has a narrow responsibility, so the symptom usually points straight at the file.

**The code:**

No single snippet here — instead, the decision procedure:

1. **Wrong status code or response shape?** Start in `Api/Endpoints/<Feature>Endpoints.cs`. The endpoint chose it.
2. **Request rejected before your code runs?** Check `Api/Filters/ValidationFilter.cs` (400s) and the antiforgery filter (also 400s) — see Byte 8.
3. **Wrong totals, wrong rules?** Go to `Application/Services/` or `Application/Pricing/PriceCalculator.cs` — see Bytes 11–12.
4. **Wrong or missing data?** Go to `Infrastructure/Repositories/Repositories.cs` — the LINQ queries live there, see Byte 14.
5. **App won't start / migration errors?** Look at the top of `Api/Program.cs` — database initialization runs before the pipeline, see Byte 24.
6. **Login or cookie weirdness?** `Api/Extensions/ServiceExtensions.cs` holds all Identity, cookie, session, and antiforgery options — see Bytes 16–17.

Because `Domain` has no dependencies and `Application` has no I/O, most business-logic bugs can be reproduced in the unit tests under `tests/LegacyEcom.Tests/Services/` without running the app at all.

## PUTTING IT TOGETHER

The project structure is a physical expression of the layered architecture: each folder answers "what kind of concern lives here." Features are named consistently across endpoints, services, and repositories, so tracing a feature vertically is mechanical. When debugging, match the symptom to the layer's responsibility and start there — the codebase is organized to make that first guess correct most of the time.
