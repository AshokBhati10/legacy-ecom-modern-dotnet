# Database

## Engine and database

- SQL Server (verified on 2022 Express, port 1433).
- Modern database: **`LegacyEcommerceModernDb`** — deliberately separate from
  the legacy `LegacyEcommerceDb`, so the original data is never touched.

## Schema

EF Core code-first, one `IEntityTypeConfiguration<T>` per entity. Legacy
commerce table/column names, keys, relationships, string lengths, nullability,
and decimal precision are preserved:

- `Categories` (self-referencing `ParentCategoryId` hierarchy)
- `Products` (SKU, slug, price/sale price, stock, flags, `CategoryId`)
- `ProductImages`, `ProductVariants` (price adjustment, per-variant stock)
- `Customers` (profile + link to Identity user by email)
- `Addresses`
- `CartItems` (persistent cart for authenticated users)
- `Orders` (`OrderNumber`, snapshot address fields, subtotal/shipping/tax/total,
  payment method, status), `OrderLines` (snapshot product name/SKU/unit price)

Plus the ASP.NET Core Identity tables (`AspNetUsers`, `AspNetRoles`,
`AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserRoles`, `AspNetUserTokens`).

## Migrations

- Initial migration: `src/LegacyEcom.Infrastructure/Data/Migrations/20261005143635_InitialCreate.cs`
- Apply: `dotnet ef database update --project src/LegacyEcom.Infrastructure --startup-project src/LegacyEcom.Api`
- The API also applies pending migrations automatically at startup for
  relational providers (and `EnsureCreated` for non-relational/test providers).

## Seed data

`DbSeeder` ports the legacy seed catalog: **6 categories, 12 products,
17 images, 5 variants**. It runs automatically when `SeedData:Enabled` is true
(`appsettings.Development.json`) and is idempotent.

## Configuration

Connection string key: `ConnectionStrings:EcommerceDb`. Provide it via the
environment — never in a committed file:

```bash
export ConnectionStrings__EcommerceDb="Server=localhost;Database=LegacyEcommerceModernDb;User Id=sa;Password=<pwd>;TrustServerCertificate=True;Encrypt=True"
```

`appsettings.json` and `appsettings.example.json` ship with placeholder
values only. `bin/`, `obj/`, and any local secrets are git-ignored and never
committed.

## Test databases

- Automated API tests use an isolated EF Core **InMemory** database per test
  run (no SQL Server needed, no cross-test interference).
- The 45-check E2E suite runs against the real `LegacyEcommerceModernDb`.
