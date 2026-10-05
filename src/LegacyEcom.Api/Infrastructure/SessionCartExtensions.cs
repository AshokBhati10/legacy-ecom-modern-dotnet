using System.Text.Json;
using LegacyEcom.Application.DTOs.Cart;

namespace LegacyEcom.Api.Infrastructure;

/// <summary>
/// Session-backed cart storage. Mirrors the legacy <c>SessionCartHelper</c>:
/// anonymous shoppers keep their items in server-side session (25-minute
/// timeout); authenticated shoppers additionally persist to the CartItems
/// table via <c>ICartService.SaveForUser</c>.
/// </summary>
public static class SessionCartExtensions
{
    private const string CartKey = "Cart";
    private const string CheckoutAddressKey = "Checkout.Address";
    private const string CheckoutShippingKey = "Checkout.ShippingMethod";
    private const string LastOrderKey = "Checkout.LastOrderId";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static List<SessionCartItem> GetCartItems(this ISession session)
    {
        var json = session.GetString(CartKey);
        if (string.IsNullOrEmpty(json)) return new List<SessionCartItem>();
        try
        {
            return JsonSerializer.Deserialize<List<SessionCartItem>>(json, JsonOptions)
                   ?? new List<SessionCartItem>();
        }
        catch (JsonException)
        {
            return new List<SessionCartItem>();
        }
    }

    public static void SetCartItems(this ISession session, IEnumerable<SessionCartItem> items) =>
        session.SetString(CartKey, JsonSerializer.Serialize(items.ToList(), JsonOptions));

    public static void ClearCart(this ISession session) => session.Remove(CartKey);

    public static void SetCheckoutAddress(this ISession session, object address) =>
        session.SetString(CheckoutAddressKey, JsonSerializer.Serialize(address, JsonOptions));

    public static T? GetCheckoutAddress<T>(this ISession session)
    {
        var json = session.GetString(CheckoutAddressKey);
        if (string.IsNullOrEmpty(json)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    public static void SetCheckoutShippingMethod(this ISession session, string method) =>
        session.SetString(CheckoutShippingKey, method);

    public static string? GetCheckoutShippingMethod(this ISession session) =>
        session.GetString(CheckoutShippingKey);

    public static void ClearCheckoutState(this ISession session)
    {
        session.Remove(CheckoutAddressKey);
        session.Remove(CheckoutShippingKey);
    }

    public static void SetLastOrderId(this ISession session, int orderId) =>
        session.SetInt32(LastOrderKey, orderId);

    public static int? GetLastOrderId(this ISession session) =>
        session.GetInt32(LastOrderKey);
}
