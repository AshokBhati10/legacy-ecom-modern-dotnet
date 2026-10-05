using LegacyEcom.Domain.Entities;
using LegacyEcom.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LegacyEcom.Tests.Api;

/// <summary>
/// Test host: the real API wired to an isolated EF Core InMemory database
/// instead of SQL Server. Seeds a small catalog on first use.
/// </summary>
public class EcommerceWebFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the SQL Server DbContext registration from Program.cs.
            foreach (var d in services
                .Where(d => d.ServiceType == typeof(DbContextOptions<EcommerceDbContext>)
                         || d.ServiceType == typeof(EcommerceDbContext))
                .ToList())
                services.Remove(d);

            // Explicit singleton options: one fixed InMemory store per factory
            // instance, shared by every scope (AddDbContext's re-registration
            // does not reliably stay singleton under WebApplicationFactory).
            var storeName = $"e2e-{Guid.NewGuid()}";
            services.AddSingleton(_ =>
            {
                var builder = new DbContextOptionsBuilder<EcommerceDbContext>();
                builder.UseInMemoryDatabase(storeName);
                return builder.Options;
            });
            services.AddScoped<EcommerceDbContext>();
        });
    }

    public void SeedCatalog()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EcommerceDbContext>();
        if (db.Categories.Any()) return;

        var cat = new Category { Id = 1, Name = "Audio", Slug = "audio", DisplayOrder = 1, IsActive = true };
        db.Categories.Add(cat);
        db.Products.Add(new Product
        {
            Id = 1, Sku = "AUD-HEAD-001", Name = "Wireless Headphones Pro",
            Slug = "wireless-headphones-pro", Price = 149.99m, SalePrice = 119.99m,
            CategoryId = 1, IsActive = true, IsFeatured = true,
            StockQuantity = 42, CreatedDate = DateTime.UtcNow,
            Variants = new List<ProductVariant>
            {
                new() { Id = 1, Name = "Midnight Black", PriceAdjustment = 0m, StockQuantity = 25, IsActive = true }
            }
        });
        db.Products.Add(new Product
        {
            Id = 2, Sku = "AUD-SPK-002", Name = "Portable Bluetooth Speaker",
            Slug = "portable-bluetooth-speaker", Price = 59.99m,
            CategoryId = 1, IsActive = true, StockQuantity = 85,
            CreatedDate = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}
