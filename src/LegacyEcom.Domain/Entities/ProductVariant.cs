namespace LegacyEcom.Domain.Entities;

/// <summary>
/// Product variant (e.g. size/color option). Adjusts the product's effective
/// price by <see cref="PriceAdjustment"/>; may carry its own SKU.
/// </summary>
public class ProductVariant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public decimal PriceAdjustment { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }

    public Product? Product { get; set; }
}
