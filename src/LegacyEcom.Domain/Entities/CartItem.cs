namespace LegacyEcom.Domain.Entities;

/// <summary>
/// Persisted cart line for an authenticated user. Anonymous shoppers keep
/// their cart in server-side session instead (see <c>CartEndpoints</c>),
/// exactly like the legacy application's session cart.
/// </summary>
public class CartItem
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public int? VariantId { get; set; }
    public int Quantity { get; set; }
    public DateTime DateCreated { get; set; }
}
