using LegacyEcom.Application.Pricing;
using Xunit;

namespace LegacyEcom.Tests.Pricing;

/// <summary>
/// Unit tests for the pricing rules ported 1:1 from the legacy application:
/// Standard $4.99 (free at $50+), Express $12.99, tax 8%.
/// </summary>
public class PriceCalculatorTests
{
    [Theory]
    [InlineData(10.00, 2, 20.00)]
    [InlineData(119.99, 1, 119.99)]
    [InlineData(9.995, 1, 10.00)] // rounds to 2 decimals
    [InlineData(5.00, 0, 0.00)]
    public void CalculateLineTotal_MultipliesAndRounds(decimal unitPrice, int quantity, decimal expected) =>
        Assert.Equal(expected, PriceCalculator.CalculateLineTotal(unitPrice, quantity));

    [Fact]
    public void CalculateShippingCost_StandardIsFreeAtThreshold()
    {
        Assert.Equal(0m, PriceCalculator.CalculateShippingCost(50.00m, "Standard"));
        Assert.Equal(0m, PriceCalculator.CalculateShippingCost(120.00m, "Standard"));
        Assert.Equal(4.99m, PriceCalculator.CalculateShippingCost(49.99m, "Standard"));
    }

    [Fact]
    public void CalculateShippingCost_ExpressIsFlat()
    {
        Assert.Equal(12.99m, PriceCalculator.CalculateShippingCost(1000m, "Express"));
        Assert.Equal(12.99m, PriceCalculator.CalculateShippingCost(0m, "Express"));
    }

    [Fact]
    public void CalculateShippingCost_UnknownMethodFallsBackToStandard() =>
        Assert.Equal(4.99m, PriceCalculator.CalculateShippingCost(10m, "Overnight"));

    [Fact]
    public void CalculateTax_IsEightPercent()
    {
        Assert.Equal(8.00m, PriceCalculator.CalculateTax(100m));
        Assert.Equal(10.40m, PriceCalculator.CalculateTax(129.99m));
        Assert.Equal(0m, PriceCalculator.CalculateTax(0m));
    }

    [Fact]
    public void CalculateTotal_SumsComponents() =>
        Assert.Equal(153.38m, PriceCalculator.CalculateTotal(129.99m, 12.99m, 10.40m));

    [Fact]
    public void NormalizeShippingMethod_MapsCorrectly()
    {
        Assert.Equal("Express", PriceCalculator.NormalizeShippingMethod("express"));
        Assert.Equal("Express", PriceCalculator.NormalizeShippingMethod("Express"));
        Assert.Equal("Standard", PriceCalculator.NormalizeShippingMethod("Standard"));
        Assert.Equal("Standard", PriceCalculator.NormalizeShippingMethod(null));
        Assert.Equal("Standard", PriceCalculator.NormalizeShippingMethod("Bogus"));
    }

    [Fact]
    public void GetShippingOptions_ReturnsBothOptionsWithCorrectCosts()
    {
        var options = PriceCalculator.GetShippingOptions(60m);
        Assert.Equal(2, options.Count);
        var standard = Assert.Single(options, o => o.Code == "Standard");
        var express = Assert.Single(options, o => o.Code == "Express");
        Assert.Equal(0m, standard.Cost); // free at threshold
        Assert.Equal(12.99m, express.Cost);
        Assert.Contains("FREE", standard.Description);
    }
}
