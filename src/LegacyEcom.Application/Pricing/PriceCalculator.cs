using LegacyEcom.Application.DTOs.Cart;
using LegacyEcom.Application.DTOs.Checkout;

namespace LegacyEcom.Application.Pricing;

/// <summary>
/// Pricing calculation logic. Pure functions; no I/O.
/// Ported 1:1 from the legacy <c>Ecommerce.Services.Pricing.PriceCalculator</c>:
/// Standard shipping $4.99 (free at $50+), Express $12.99, tax 8%.
/// </summary>
public static class PriceCalculator
{
    public const string StandardShippingCode = "Standard";
    public const string ExpressShippingCode = "Express";

    private const decimal StandardShippingFlatRate = 4.99m;
    private const decimal ExpressShippingFlatRate = 12.99m;
    private const decimal FreeShippingThreshold = 50.00m;
    private const decimal TaxRate = 0.08m;

    public static decimal CalculateLineTotal(decimal unitPrice, int quantity)
    {
        if (quantity < 0) quantity = 0;
        return Math.Round(unitPrice * quantity, 2);
    }

    public static decimal CalculateSubTotal(IEnumerable<CartLineDto> lines)
    {
        if (lines is null) return 0m;
        return Math.Round(lines.Sum(l => l.LineTotal), 2);
    }

    public static decimal CalculateShippingCost(decimal subTotal, string? shippingMethod)
    {
        if (string.Equals(shippingMethod, ExpressShippingCode, StringComparison.OrdinalIgnoreCase))
            return ExpressShippingFlatRate;

        // Standard: free once the threshold is reached.
        return subTotal >= FreeShippingThreshold ? 0m : StandardShippingFlatRate;
    }

    public static decimal CalculateTax(decimal subTotal)
    {
        if (subTotal < 0) subTotal = 0;
        return Math.Round(subTotal * TaxRate, 2);
    }

    public static decimal CalculateTotal(decimal subTotal, decimal shippingCost, decimal taxAmount) =>
        Math.Round(subTotal + shippingCost + taxAmount, 2);

    public static IReadOnlyList<ShippingOptionDto> GetShippingOptions(decimal subTotal) =>
        new List<ShippingOptionDto>
        {
            new(
                StandardShippingCode,
                "Standard",
                "3-5 business days" + (subTotal >= FreeShippingThreshold ? " — FREE" : ""),
                CalculateShippingCost(subTotal, StandardShippingCode)),
            new(
                ExpressShippingCode,
                "Express",
                "1-2 business days",
                CalculateShippingCost(subTotal, ExpressShippingCode))
        };

    public static string NormalizeShippingMethod(string? shippingMethod) =>
        string.Equals(shippingMethod, ExpressShippingCode, StringComparison.OrdinalIgnoreCase)
            ? ExpressShippingCode
            : StandardShippingCode;
}
