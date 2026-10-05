namespace LegacyEcom.Domain.Entities;

/// <summary>
/// Placed order. Shipping details are snapshotted onto the order at placement
/// time (denormalized <c>Ship*</c> columns), so later address edits cannot
/// rewrite history — same as the legacy schema.
/// </summary>
public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public string? UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;

    public decimal SubTotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }

    public string ShippingMethod { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;

    public string ShipFirstName { get; set; } = string.Empty;
    public string ShipLastName { get; set; } = string.Empty;
    public string ShipEmail { get; set; } = string.Empty;
    public string? ShipPhone { get; set; }
    public string ShipStreet { get; set; } = string.Empty;
    public string ShipCity { get; set; } = string.Empty;
    public string ShipState { get; set; } = string.Empty;
    public string ShipPostalCode { get; set; } = string.Empty;
    public string ShipCountry { get; set; } = string.Empty;

    public Customer? Customer { get; set; }
    public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
}
