# 07 — Database and EF Core

### Byte 23: EcommerceDbContext and code-first configuration

**Builds on:** Byte 14

**In plain terms:**

`EcommerceDbContext` is EF Core's gateway to SQL Server: a `DbSet<T>` per table plus mapping classes that declare table names, column types, lengths, indexes, and relationships in code. Table and column names intentionally match the legacy database schema so the data model stays recognizable.

**The code:**

```csharp
// Data/EcommerceDbContext.cs
public class EcommerceDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    // ...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);   // Identity tables first
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcommerceDbContext).Assembly);
    }
}
```

```csharp
// Data/Configurations/CatalogConfigurations.cs — one class per entity.
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products");
        b.HasKey(x => x.Id);
        b.Property(x => x.Sku).HasMaxLength(50).IsRequired();
        b.Property(x => x.Price).HasColumnType("decimal(18,2)");
        b.HasIndex(x => x.Slug).IsUnique().HasDatabaseName("IX_Products_Slug");

        b.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

Inheriting `IdentityDbContext<AppUser>` adds the ASP.NET Core Identity tables (`AspNetUsers`, `AspNetRoles`, …) alongside the store tables in one database. If a column looks wrong in the database, the `Configurations` folder — not the entity class — is where the mapping is decided.

### Byte 24: Migrations, startup, and seeding

**Builds on:** Byte 23

**In plain terms:**

The database schema is versioned as EF Core migrations (timestamped C# files under `Data/Migrations`). On startup the app applies any pending migrations to SQL Server automatically; for non-relational providers (the in-memory database used by tests) it just creates the schema. In Development, a seeder then inserts the demo catalog — but only when `SeedData:Enabled` is true.

**The code:**

```csharp
// Program.cs — runs before the web pipeline starts.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EcommerceDbContext>();
    if (db.Database.IsRelational())
        await db.Database.MigrateAsync();      // SQL Server: real migrations
    else
        await db.Database.EnsureCreatedAsync(); // tests: create schema directly

    if (app.Configuration.GetValue<bool>("SeedData:Enabled"))
        await DbSeeder.SeedAsync(db, logger);   // idempotent demo catalog
}
```

```json
// appsettings.json — seeding is OFF by default; the example turns it on.
{ "SeedData": { "Enabled": false } }
```

Practical consequences: a fresh database only needs the connection string — first startup builds everything. And because seeding is idempotent and opt-in, production never gets demo products by accident. If startup fails, this block is the first suspect (check the connection string and SQL Server reachability).

### Byte 25: Gotchas — the sharp edges

**Builds on:** Bytes 23–24

**In plain terms:**

Three non-obvious behaviors that have bitten before. First, the test factory must register `DbContextOptions` as a **singleton** with one fixed in-memory store name — re-registering via `AddDbContext` produced a *new* options instance per scope, so seeded data was invisible across scopes. Second, `AddDbContext` is scoped-per-request in production, which is what makes one request equal one unit of work (Byte 15) — don't fight it. Third, session state (cart, checkout wizard) dies after 25 idle minutes by design; "my cart vanished" is usually just the timeout, not a bug.

**The code:**

```csharp
// tests/LegacyEcom.Tests/Api/EcommerceWebFactory.cs — the working pattern.
var storeName = $"e2e-{Guid.NewGuid()}";
services.AddSingleton(_ =>
{
    var builder = new DbContextOptionsBuilder<EcommerceDbContext>();
    builder.UseInMemoryDatabase(storeName);   // one store per factory instance
    return builder.Options;
});
services.AddScoped<EcommerceDbContext>();
```

```csharp
// ServiceExtensions.cs — the 25-minute session both carts and auth share.
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(25);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

If integration tests suddenly see empty tables, check the `DbContextOptions` lifetime first. If a user reports a lost cart or a 409 mid-checkout after a coffee break, check the clock before the code.

## PUTTING IT TOGETHER

The database is described entirely in code: entities in `Domain`, mappings in `Infrastructure/Data/Configurations`, versions in `Migrations`, applied automatically at startup with opt-in seeding. Repositories translate service calls into LINQ; EF Core translates LINQ into SQL against `LegacyEcommerceModernDb`. The sharp edges are all about lifetimes — scoped `DbContext` per request, singleton options in tests, 25-minute session — and each has exactly one place where it's decided.
