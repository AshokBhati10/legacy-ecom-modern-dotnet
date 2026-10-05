namespace LegacyEcom.Domain.Entities;

/// <summary>
/// Order line. Product/variant names, SKU and unit price are snapshotted at
/// placement time so catalog edits never rewrite order history.
/// </summary>
public class OrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int? VariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? VariantName { get; set; }
    public string? Sku { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }

    public Order? Order { get; set; }
}
