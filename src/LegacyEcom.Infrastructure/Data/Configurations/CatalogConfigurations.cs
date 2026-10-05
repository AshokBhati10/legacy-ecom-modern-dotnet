using LegacyEcom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacyEcom.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.Sku).HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Slug).HasMaxLength(200).IsRequired();
        b.Property(x => x.ShortDescription).HasMaxLength(500);
        b.Property(x => x.Description).HasColumnType("nvarchar(max)");
        b.Property(x => x.Price).HasColumnType("decimal(18,2)");
        b.Property(x => x.SalePrice).HasColumnType("decimal(18,2)");
        b.Property(x => x.ThumbnailUrl).HasMaxLength(500);
        b.HasIndex(x => x.Slug).IsUnique().HasDatabaseName("IX_Products_Slug");
        b.HasIndex(x => x.CategoryId).HasDatabaseName("IX_Products_CategoryId");
        b.HasIndex(x => x.IsActive).HasDatabaseName("IX_Products_IsActive");

        b.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Images)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.Variants)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Ignore(x => x.EffectivePrice);
    }
}

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> b)
    {
        b.ToTable("ProductImages");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.Url).HasMaxLength(500).IsRequired();
        b.Property(x => x.AltText).HasMaxLength(200);
        b.HasIndex(x => x.ProductId).HasDatabaseName("IX_ProductImages_ProductId");
    }
}

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> b)
    {
        b.ToTable("ProductVariants");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Sku).HasMaxLength(50);
        b.Property(x => x.PriceAdjustment).HasColumnType("decimal(18,2)");
        b.HasIndex(x => x.ProductId).HasDatabaseName("IX_ProductVariants_ProductId");
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.ToTable("Categories");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Slug).HasMaxLength(200).IsRequired();
        b.Property(x => x.Description).HasColumnType("nvarchar(max)");
        b.HasIndex(x => x.Slug).IsUnique().HasDatabaseName("IX_Categories_Slug");

        b.HasOne(x => x.ParentCategory)
            .WithMany(x => x.ChildCategories)
            .HasForeignKey(x => x.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
