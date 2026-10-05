using LegacyEcom.Application.DTOs.Cart;
using LegacyEcom.Application.Services;
using LegacyEcom.Infrastructure.Repositories;
using Xunit;

namespace LegacyEcom.Tests.Services;

public class CartServiceTests
{
    private static CartService CreateService(out Infrastructure.Data.EcommerceDbContext db)
    {
        db = TestDb.Create();
        return new CartService(
            new ProductRepository(db),
            new CartRepository(db),
            new UnitOfWork(db));
    }

    [Fact]
    public void AddItem_AddsLineWithEffectivePrice()
    {
        var cart = CreateService(out _);

        var result = cart.AddItem(Array.Empty<SessionCartItem>(), 1, null, 1);

        Assert.True(result.Success);
        var dto = cart.GetCart(result.Data!);
        Assert.Equal(1, dto.ItemCount);
        Assert.Equal(119.99m, dto.Items[0].UnitPrice); // sale price wins
        Assert.Equal(119.99m, dto.SubTotal);
    }

    [Fact]
    public void AddItem_WithVariant_AppliesPriceAdjustment()
    {
        var cart = CreateService(out _);

        var result = cart.AddItem(Array.Empty<SessionCartItem>(), 1, 2, 1);

        Assert.True(result.Success);
        var line = cart.GetCart(result.Data!).Items[0];
        Assert.Equal("Arctic Silver", line.VariantName);
        Assert.Equal(129.99m, line.UnitPrice); // 119.99 + 10.00
    }

    [Fact]
    public void AddItem_ExistingLine_AccumulatesUpToMax()
    {
        var cart = CreateService(out _);

        var first = cart.AddItem(Array.Empty<SessionCartItem>(), 2, null, 90);
        var second = cart.AddItem(first.Data!, 2, null, 20);

        Assert.True(second.Success);
        Assert.Equal(99, cart.GetCart(second.Data!).ItemCount); // capped at 99
    }

    [Fact]
    public void AddItem_UnknownProduct_Fails()
    {
        var cart = CreateService(out _);

        var result = cart.AddItem(Array.Empty<SessionCartItem>(), 999, null, 1);

        Assert.False(result.Success);
        Assert.Equal("Product not found.", result.Message);
    }

    [Fact]
    public void AddItem_UnknownVariant_Fails()
    {
        var cart = CreateService(out _);

        var result = cart.AddItem(Array.Empty<SessionCartItem>(), 1, 999, 1);

        Assert.False(result.Success);
        Assert.Equal("Product variant not found.", result.Message);
    }

    [Fact]
    public void UpdateItem_ZeroQuantity_RemovesLine()
    {
        var cart = CreateService(out _);
        var added = cart.AddItem(Array.Empty<SessionCartItem>(), 1, null, 2);

        var updated = cart.UpdateItem(added.Data!, 1, null, 0);

        Assert.True(updated.Success);
        Assert.Empty(cart.GetCart(updated.Data!).Items);
    }

    [Fact]
    public void UpdateItem_MissingLine_Fails()
    {
        var cart = CreateService(out _);

        var result = cart.UpdateItem(Array.Empty<SessionCartItem>(), 1, null, 2);

        Assert.False(result.Success);
    }

    [Fact]
    public void GetCart_SkipsInactiveProducts()
    {
        var cart = CreateService(out _);

        var dto = cart.GetCart(new[] { new SessionCartItem(3, null, 1) });

        Assert.Empty(dto.Items);
        Assert.Equal(0m, dto.SubTotal);
    }

    [Fact]
    public void SaveAndLoadForUser_RoundTrips()
    {
        var cart = CreateService(out _);
        var items = new[] { new SessionCartItem(1, 2, 3) };

        cart.SaveForUser("user-1", items);
        var loaded = cart.LoadForUser("user-1");

        var loadedItem = Assert.Single(loaded);
        Assert.Equal(1, loadedItem.ProductId);
        Assert.Equal(2, loadedItem.VariantId);
        Assert.Equal(3, loadedItem.Quantity);
    }
}
