namespace LegacyEcom.Domain.Entities;

/// <summary>
/// Catalog product. Persistence-ignorant POCO; mapped by EF Core in
/// <c>LegacyEcom.Infrastructure</c>. Never exposed directly by the API.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int CategoryId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedDate { get; set; }

    public Category? Category { get; set; }
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

    /// <summary>Effective selling price (sale price wins when present and positive).</summary>
    public decimal EffectivePrice =>
        SalePrice.HasValue && SalePrice.Value > 0 ? SalePrice.Value : Price;
}
