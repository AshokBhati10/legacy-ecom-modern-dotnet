using LegacyEcom.Domain.Entities;
using LegacyEcom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyEcom.Tests.Services;

/// <summary>
/// Builds an isolated InMemory <see cref="EcommerceDbContext"/> seeded with a
/// small catalog for service-level tests.
/// </summary>
public static class TestDb
{
    public static EcommerceDbContext Create(string? name = null)
    {
        var options = new DbContextOptionsBuilder<EcommerceDbContext>()
            .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
            .Options;

        var db = new EcommerceDbContext(options);
        SeedCatalog(db);
        return db;
    }

    private static void SeedCatalog(EcommerceDbContext db)
    {
        var cat = new Category
        {
            Id = 1, Name = "Audio", Slug = "audio",
            DisplayOrder = 1, IsActive = true
        };
        db.Categories.Add(cat);

        var headphones = new Product
        {
            Id = 1, Sku = "AUD-HEAD-001", Name = "Wireless Headphones Pro",
            Slug = "wireless-headphones-pro", Price = 149.99m, SalePrice = 119.99m,
            CategoryId = 1, IsActive = true, IsFeatured = true,
            StockQuantity = 42, CreatedDate = DateTime.UtcNow,
            Images = new List<ProductImage>
            {
                new() { Id = 1, Url = "/img/1.jpg", DisplayOrder = 1, IsMain = true }
            },
            Variants = new List<ProductVariant>
            {
                new() { Id = 1, Name = "Midnight Black", Sku = "AUD-HEAD-001-BLK", PriceAdjustment = 0m, StockQuantity = 25, IsActive = true },
                new() { Id = 2, Name = "Arctic Silver", Sku = "AUD-HEAD-001-SLV", PriceAdjustment = 10m, StockQuantity = 17, IsActive = true }
            }
        };
        var speaker = new Product
        {
            Id = 2, Sku = "AUD-SPK-002", Name = "Portable Bluetooth Speaker",
            Slug = "portable-bluetooth-speaker", Price = 59.99m,
            CategoryId = 1, IsActive = true, StockQuantity = 85,
            CreatedDate = DateTime.UtcNow
        };
        var inactive = new Product
        {
            Id = 3, Sku = "OLD-001", Name = "Discontinued",
            Slug = "discontinued", Price = 9.99m,
            CategoryId = 1, IsActive = false, StockQuantity = 0,
            CreatedDate = DateTime.UtcNow
        };
        db.Products.AddRange(headphones, speaker, inactive);
        db.SaveChanges();
        db.ChangeTracker.Clear();
    }
}
