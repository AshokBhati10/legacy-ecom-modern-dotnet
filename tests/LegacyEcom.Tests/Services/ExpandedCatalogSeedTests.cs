using LegacyEcom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace LegacyEcom.Tests.Services;

/// <summary>
/// Verifies the expanded catalog seeder: ~500 products, idempotent,
/// unique slugs/SKUs, valid category distribution, and image files exist.
/// </summary>
public class ExpandedCatalogSeedTests
{
    private static EcommerceDbContext CreateDb(string name)
    {
        var options = new DbContextOptionsBuilder<EcommerceDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new EcommerceDbContext(options);
    }

    [Fact]
    public async Task SeedAsync_CreatesApproximately500Products()
    {
        using var db = CreateDb(Guid.NewGuid().ToString());
        await DbSeeder.SeedAsync(db, NullLogger.Instance);

        var count = await db.Products.CountAsync();
        Assert.True(count >= 495 && count <= 505,
            $"Expected ~500 products, got {count}");
    }

    [Fact]
    public async Task SeedAsync_IsIdempotent()
    {
        using var db = CreateDb(Guid.NewGuid().ToString());
        await DbSeeder.SeedAsync(db, NullLogger.Instance);
        var first = await db.Products.CountAsync();

        await DbSeeder.SeedAsync(db, NullLogger.Instance);
        var second = await db.Products.CountAsync();

        Assert.Equal(first, second);
    }

    [Fact]
    public async Task SeedAsync_PreservesOriginal12Products()
    {
        using var db = CreateDb(Guid.NewGuid().ToString());
        await DbSeeder.SeedAsync(db, NullLogger.Instance);

        var originalSkus = new[]
        {
            "AUD-HEAD-001", "AUD-SPK-002", "ELE-LAP-003", "ELE-PHN-004",
            "HMK-CFM-005", "HMK-KNF-006", "SPO-YGA-007", "SPO-RUN-008",
            "SPO-DMB-009", "BOK-CLS-010", "CLT-DJK-011", "CLT-TEE-012"
        };
        foreach (var sku in originalSkus)
        {
            Assert.True(await db.Products.AnyAsync(p => p.Sku == sku),
                $"Original product {sku} missing");
        }
    }

    [Fact]
    public async Task SeedAsync_DistributesAcrossCategories()
    {
        using var db = CreateDb(Guid.NewGuid().ToString());
        await DbSeeder.SeedAsync(db, NullLogger.Instance);

        var byCategory = await db.Products
            .GroupBy(p => p.Category.Slug)
            .Select(g => new { Slug = g.Key, Count = g.Count() })
            .ToListAsync();

        // Each category should have a meaningful share (at least 20 products).
        foreach (var c in byCategory)
        {
            Assert.True(c.Count >= 20,
                $"Category {c.Slug} has only {c.Count} products");
        }
    }

    [Fact]
    public async Task SeedAsync_AllSlugsAndSkusUnique()
    {
        using var db = CreateDb(Guid.NewGuid().ToString());
        await DbSeeder.SeedAsync(db, NullLogger.Instance);

        var slugs = await db.Products.Select(p => p.Slug).ToListAsync();
        var skus = await db.Products.Select(p => p.Sku).ToListAsync();

        Assert.Equal(slugs.Count, slugs.Distinct().Count());
        Assert.Equal(skus.Count, skus.Distinct().Count());
    }

    [Fact]
    public async Task SeedAsync_EveryProductHasValidImagePath()
    {
        using var db = CreateDb(Guid.NewGuid().ToString());
        await DbSeeder.SeedAsync(db, NullLogger.Instance);

        var products = await db.Products
            .Select(p => new { p.Sku, p.ThumbnailUrl })
            .ToListAsync();

        foreach (var p in products)
        {
            Assert.False(string.IsNullOrWhiteSpace(p.ThumbnailUrl),
                $"Product {p.Sku} has no thumbnail");
            Assert.StartsWith("/Content/images/products/", p.ThumbnailUrl);
            Assert.EndsWith(".jpg", p.ThumbnailUrl);
        }

        // Every referenced image file must exist in the frontend.
        var files = products
            .Select(p => p.ThumbnailUrl!.Replace("/Content/images/products/", ""))
            .Distinct()
            .ToList();
        var productsDir = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "..",
            "frontend", "images", "products"));
        foreach (var f in files)
        {
            Assert.True(File.Exists(Path.Combine(productsDir, f)),
                $"Image file missing: {f}");
        }
    }

    [Fact]
    public async Task SeedAsync_RealisticNames_NoPlaceholders()
    {
        using var db = CreateDb(Guid.NewGuid().ToString());
        await DbSeeder.SeedAsync(db, NullLogger.Instance);

        var names = await db.Products.Select(p => p.Name).ToListAsync();
        foreach (var name in names)
        {
            Assert.DoesNotContain("Product 00", name);
            Assert.DoesNotContain("Test Product", name);
            Assert.DoesNotContain("Sample Product", name);
            Assert.DoesNotContain("Demo Product", name);
        }
    }
}
