using LegacyEcom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LegacyEcom.Infrastructure.Data;

/// <summary>
/// Idempotent development seeder. Ports the legacy <c>Database/SeedData.sql</c>
/// (6 categories, 12 products, 17 images, 5 variants) into EF Core, then
/// expands the catalog to ~500 products via <c>ExpandedCatalog</c>.
/// Runs only when enabled via configuration (<c>SeedData:Enabled</c>,
/// default on in Development). The expansion is SKU-idempotent: re-running
/// adds only products whose SKU is not already present.
/// </summary>
public static class DbSeeder
{
    private const int TargetProductCount = 500;

    public static async Task SeedAsync(EcommerceDbContext db, ILogger logger, CancellationToken ct = default)
    {
        if (!await db.Categories.AnyAsync(ct))
        {
            await SeedCategoriesAsync(db, ct);
        }

        var existingCount = await db.Products.CountAsync(ct);
        if (existingCount >= TargetProductCount)
        {
            logger.LogInformation("Seed data already contains {Count} products; skipping product generation.", existingCount);
            return;
        }

        // Ensure the original 12 products exist (in case categories were
        // seeded but products were not, e.g. partial historical state).
        var existingSkus = new HashSet<string>(
            await db.Products.Where(p => p.Sku != null).Select(p => p.Sku!).ToListAsync(ct));

        var baseProductsAdded = 0;
        if (existingSkus.Count == 0)
        {
            baseProductsAdded = await SeedBaseProductsAsync(db, ct);
            foreach (var p in await db.Products.Select(p => p.Sku).ToListAsync(ct))
                existingSkus.Add(p);
        }

        var expanded = ExpandedCatalog.GetAll();
        var missing = expanded.Where(p => !existingSkus.Contains(p.Sku)).ToList();

        // Only add what is needed to reach the target.
        var needed = TargetProductCount - (existingCount + baseProductsAdded);
        var toAdd = missing.Take(Math.Max(0, needed)).ToList();

        if (toAdd.Count == 0)
        {
            logger.LogInformation("Seed data already contains {Count} products; skipping product generation.", existingCount + baseProductsAdded);
            return;
        }

        var categoriesBySlug = await db.Categories
            .ToDictionaryAsync(c => c.Slug, c => c.Id, ct);

        var products = new List<Product>(toAdd.Count);
        var images = new List<ProductImage>(toAdd.Count);
        var variants = new List<ProductVariant>();

        foreach (var def in toAdd)
        {
            var product = new Product
            {
                Sku = def.Sku,
                Name = def.Name,
                Slug = def.Slug,
                ShortDescription = def.ShortDescription,
                Description = def.Description,
                Price = def.Price,
                SalePrice = def.SalePrice,
                CategoryId = categoriesBySlug[def.CategorySlug],
                ThumbnailUrl = $"/Content/images/products/{def.ImageFile}",
                IsActive = true,
                IsFeatured = def.IsFeatured,
                StockQuantity = def.Stock,
                CreatedDate = DateTime.UtcNow
            };
            products.Add(product);
        }

        db.Products.AddRange(products);
        await db.SaveChangesAsync(ct);

        // One main image per product + a few variants for clothing to
        // demonstrate the existing variant functionality.
        var variantSkus = new HashSet<string>(
            await db.ProductVariants.Select(v => v.Sku).ToListAsync(ct));
        foreach (var product in products)
        {
            var def = toAdd[products.IndexOf(product)];
            images.Add(new ProductImage
            {
                ProductId = product.Id,
                Url = $"/Content/images/products/{def.ImageFile}",
                AltText = $"{def.Name} - main image",
                DisplayOrder = 1,
                IsMain = true
            });

            if (def.CategorySlug == "clothing" && products.IndexOf(product) % 3 == 0)
            {
                var sizes = new[] { ("Small", "S"), ("Medium", "M"), ("Large", "L") };
                foreach (var (sizeName, sizeCode) in sizes)
                {
                    var vsku = $"{def.Sku}-{sizeCode}";
                    if (variantSkus.Add(vsku))
                    {
                        variants.Add(new ProductVariant
                        {
                            ProductId = product.Id,
                            Name = sizeName,
                            Sku = vsku,
                            PriceAdjustment = sizeCode == "L" ? 2.00m : 0.00m,
                            StockQuantity = def.Stock / 3,
                            IsActive = true
                        });
                    }
                }
            }
        }

        db.ProductImages.AddRange(images);
        if (variants.Count > 0)
            db.ProductVariants.AddRange(variants);
        await db.SaveChangesAsync(ct);

        var finalCount = await db.Products.CountAsync(ct);
        logger.LogInformation(
            "Seed data inserted: Categories: 6, Base products added: {Base}, Expanded products added: {Added}, " +
            "Product images added: {Images}, Variants added: {Variants}, Total products: {Total}.",
            baseProductsAdded, toAdd.Count, images.Count, variants.Count, finalCount);
    }

    private static async Task SeedCategoriesAsync(EcommerceDbContext db, CancellationToken ct)
    {
        var categories = new[]
        {
            new Category { Name = "Electronics", Slug = "electronics", Description = "Phones, computers and gadgets.", ParentCategoryId = null, DisplayOrder = 1, IsActive = true },
            new Category { Name = "Audio", Slug = "audio", Description = "Headphones, speakers and sound.", ParentCategoryId = 1, DisplayOrder = 2, IsActive = true },
            new Category { Name = "Home & Kitchen", Slug = "home-kitchen", Description = "Appliances and kitchen essentials.", ParentCategoryId = null, DisplayOrder = 3, IsActive = true },
            new Category { Name = "Sports & Outdoors", Slug = "sports-outdoors", Description = "Fitness and outdoor gear.", ParentCategoryId = null, DisplayOrder = 4, IsActive = true },
            new Category { Name = "Books", Slug = "books", Description = "Books and collections.", ParentCategoryId = null, DisplayOrder = 5, IsActive = true },
            new Category { Name = "Clothing", Slug = "clothing", Description = "Apparel for every season.", ParentCategoryId = null, DisplayOrder = 6, IsActive = true },
        };
        db.Categories.AddRange(categories);
        await db.SaveChangesAsync(ct);
    }

    private static async Task<int> SeedBaseProductsAsync(EcommerceDbContext db, CancellationToken ct)
    {
        var categories = await db.Categories.ToDictionaryAsync(c => c.Slug, c => c, ct);
        var audio = categories["audio"]; var electronics = categories["electronics"];
        var home = categories["home-kitchen"]; var sports = categories["sports-outdoors"];
        var books = categories["books"]; var clothing = categories["clothing"];

        var products = new[]
        {
            NewProduct("AUD-HEAD-001", "Wireless Headphones Pro", "wireless-headphones-pro",
                "Noise-cancelling over-ear headphones with 30-hour battery.",
                "Premium over-ear wireless headphones with active noise cancellation, Bluetooth 5.0, 30-hour battery life and a folding design for travel.",
                149.99m, 119.99m, audio.Id, "/Content/images/products/headphones-pro-1.jpg", true, 42),
            NewProduct("AUD-SPK-002", "Portable Bluetooth Speaker", "portable-bluetooth-speaker",
                "Water-resistant portable speaker with deep bass.",
                "Compact water-resistant Bluetooth speaker with 12-hour playtime, deep bass radiator and built-in microphone for calls.",
                59.99m, null, audio.Id, "/Content/images/products/bluetooth-speaker-1.jpg", false, 85),
            NewProduct("ELE-LAP-003", "Ultrabook Laptop 14", "ultrabook-laptop-14",
                "14-inch ultrabook, 16GB RAM, 512GB SSD.",
                "Thin-and-light 14-inch ultrabook with a full-HD display, 16GB RAM, 512GB SSD and all-day battery — built for work on the move.",
                999.99m, null, electronics.Id, "/Content/images/products/ultrabook-14-1.jpg", false, 18),
            NewProduct("ELE-PHN-004", "Smartphone X", "smartphone-x",
                "6.1-inch smartphone with dual camera.",
                "Smartphone X with a 6.1-inch OLED display, dual rear camera, fast charging and 128GB of storage.",
                699.99m, 649.99m, electronics.Id, "/Content/images/products/smartphone-x-1.jpg", true, 60),
            NewProduct("HMK-CFM-005", "Coffee Maker Deluxe", "coffee-maker-deluxe",
                "12-cup programmable coffee maker.",
                "12-cup programmable coffee maker with thermal carafe, brew-strength control and auto-start timer.",
                89.99m, null, home.Id, "/Content/images/products/coffee-maker-1.jpg", false, 37),
            NewProduct("HMK-KNF-006", "Chef's Knife Set (5 pc)", "chefs-knife-set",
                "Five-piece German steel knife set.",
                "Five-piece chef's knife set in high-carbon German steel with ergonomic handles and a wooden storage block.",
                129.99m, null, home.Id, "/Content/images/products/knife-set-1.jpg", false, 25),
            NewProduct("SPO-YGA-007", "Premium Yoga Mat", "premium-yoga-mat",
                "Extra-thick non-slip yoga mat.",
                "6mm extra-thick non-slip yoga mat with alignment lines, carrying strap and sweat-resistant surface.",
                29.99m, null, sports.Id, "/Content/images/products/yoga-mat-1.jpg", false, 120),
            NewProduct("SPO-RUN-008", "Road Running Shoes", "road-running-shoes",
                "Cushioned road running shoes.",
                "Lightweight cushioned road running shoes with breathable mesh upper and responsive foam midsole.",
                119.99m, 99.99m, sports.Id, "/Content/images/products/running-shoes-1.jpg", true, 54),
            NewProduct("SPO-DMB-009", "Dumbbell Set 20kg", "dumbbell-set-20kg",
                "Adjustable 20kg dumbbell pair.",
                "Pair of adjustable dumbbells up to 20kg each with knurled steel handles and quick-change plates.",
                79.99m, null, sports.Id, "/Content/images/products/dumbbell-set-1.jpg", false, 30),
            NewProduct("BOK-CLS-010", "Classic Novel Collection", "classic-novel-collection",
                "Ten timeless classics in hardcover.",
                "Boxed set of ten timeless classic novels in hardcover with ribbon markers — a shelf essential.",
                39.99m, null, books.Id, "/Content/images/products/novel-collection-1.jpg", false, 66),
            NewProduct("CLT-DJK-011", "Denim Jacket", "denim-jacket",
                "Classic fit denim jacket.",
                "Classic fit denim jacket in washed indigo with button front and chest pockets.",
                89.99m, null, clothing.Id, "/Content/images/products/denim-jacket-1.jpg", false, 40),
            NewProduct("CLT-TEE-012", "Cotton T-Shirt", "cotton-t-shirt",
                "100% cotton crew-neck t-shirt.",
                "Soft 100% cotton crew-neck t-shirt, pre-shrunk and available in three sizes.",
                19.99m, null, clothing.Id, "/Content/images/products/cotton-tshirt-1.jpg", false, 200),
        };
        db.Products.AddRange(products);
        await db.SaveChangesAsync(ct);

        var images = new (int Product, string Url, string Alt, int Order, bool Main)[]
        {
            (0, "/Content/images/products/headphones-pro-1.jpg", "Wireless Headphones Pro - front", 1, true),
            (0, "/Content/images/products/headphones-pro-2.jpg", "Wireless Headphones Pro - side", 2, false),
            (0, "/Content/images/products/headphones-pro-3.jpg", "Wireless Headphones Pro - case", 3, false),
            (1, "/Content/images/products/bluetooth-speaker-1.jpg", "Portable Bluetooth Speaker", 1, true),
            (2, "/Content/images/products/ultrabook-14-1.jpg", "Ultrabook Laptop 14 - open", 1, true),
            (2, "/Content/images/products/ultrabook-14-2.jpg", "Ultrabook Laptop 14 - side", 2, false),
            (3, "/Content/images/products/smartphone-x-1.jpg", "Smartphone X - front", 1, true),
            (3, "/Content/images/products/smartphone-x-2.jpg", "Smartphone X - back", 2, false),
            (4, "/Content/images/products/coffee-maker-1.jpg", "Coffee Maker Deluxe", 1, true),
            (5, "/Content/images/products/knife-set-1.jpg", "Chef's Knife Set", 1, true),
            (6, "/Content/images/products/yoga-mat-1.jpg", "Premium Yoga Mat", 1, true),
            (7, "/Content/images/products/running-shoes-1.jpg", "Road Running Shoes - pair", 1, true),
            (7, "/Content/images/products/running-shoes-2.jpg", "Road Running Shoes - sole", 2, false),
            (8, "/Content/images/products/dumbbell-set-1.jpg", "Dumbbell Set 20kg", 1, true),
            (9, "/Content/images/products/novel-collection-1.jpg", "Classic Novel Collection", 1, true),
            (10, "/Content/images/products/denim-jacket-1.jpg", "Denim Jacket", 1, true),
            (11, "/Content/images/products/cotton-tshirt-1.jpg", "Cotton T-Shirt", 1, true),
        };
        db.ProductImages.AddRange(images.Select(x => new ProductImage
        {
            ProductId = products[x.Product].Id,
            Url = x.Url, AltText = x.Alt, DisplayOrder = x.Order, IsMain = x.Main
        }));

        var variants = new (int Product, string Name, string Sku, decimal Adj, int Stock)[]
        {
            (0, "Midnight Black", "AUD-HEAD-001-BLK", 0.00m, 25),
            (0, "Arctic Silver", "AUD-HEAD-001-SLV", 10.00m, 17),
            (11, "Small", "CLT-TEE-012-S", 0.00m, 80),
            (11, "Medium", "CLT-TEE-012-M", 0.00m, 70),
            (11, "Large", "CLT-TEE-012-L", 2.00m, 50),
        };
        db.ProductVariants.AddRange(variants.Select(x => new ProductVariant
        {
            ProductId = products[x.Product].Id,
            Name = x.Name, Sku = x.Sku, PriceAdjustment = x.Adj,
            StockQuantity = x.Stock, IsActive = true
        }));

        await db.SaveChangesAsync(ct);
        return products.Length;
    }

    private static Product NewProduct(
        string sku, string name, string slug,
        string shortDescription, string description,
        decimal price, decimal? salePrice, int categoryId,
        string thumbnailUrl, bool isFeatured, int stock) =>
        new()
        {
            Sku = sku, Name = name, Slug = slug,
            ShortDescription = shortDescription, Description = description,
            Price = price, SalePrice = salePrice, CategoryId = categoryId,
            ThumbnailUrl = thumbnailUrl, IsActive = true, IsFeatured = isFeatured,
            StockQuantity = stock, CreatedDate = DateTime.UtcNow
        };
}
