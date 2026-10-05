using System.Text.RegularExpressions;
using LegacyEcom.Application.DTOs.Cart;
using LegacyEcom.Application.DTOs.Checkout;
using LegacyEcom.Application.Services;
using LegacyEcom.Infrastructure.Repositories;
using Xunit;

namespace LegacyEcom.Tests.Services;

public class CheckoutServiceTests
{
    private static (CheckoutService checkout, CartService cart) CreateServices()
    {
        var db = TestDb.Create();
        var products = new ProductRepository(db);
        var carts = new CartRepository(db);
        var orders = new OrderRepository(db);
        var customers = new CustomerRepository(db);
        var uow = new UnitOfWork(db);
        var cartService = new CartService(products, carts, uow);
        var checkout = new CheckoutService(cartService, orders, customers, uow);
        return (checkout, cartService);
    }

    private static AddressRequest ValidAddress() => new(
        "buyer@example.com", "Test", "Buyer", null,
        "1 Market St", "Springfield", "IL", "62701", "USA");

    [Fact]
    public void PlaceOrder_EmptyCart_Fails()
    {
        var (checkout, _) = CreateServices();

        var result = checkout.PlaceOrder(
            Array.Empty<SessionCartItem>(), ValidAddress(), "Standard", "CashOnDelivery", null);

        Assert.False(result.Success);
        Assert.Equal("Your cart is empty.", result.Message);
    }

    [Fact]
    public void PlaceOrder_CreatesOrderWithCorrectTotals()
    {
        var (checkout, cart) = CreateServices();
        var items = cart.AddItem(Array.Empty<SessionCartItem>(), 2, null, 2).Data!;
        // 2 x 59.99 = 119.98 subtotal; standard shipping free (>= 50); tax 8% = 9.60

        var result = checkout.PlaceOrder(items, ValidAddress(), "Standard", "CashOnDelivery", null);

        Assert.True(result.Success);
        var order = result.Data!;
        Assert.Matches(new Regex(@"^LE-\d{8}-[A-Z0-9]{6}$"), order.OrderNumber);
        Assert.Equal("Placed", order.Status);
        Assert.Equal(119.98m, order.SubTotal);
        Assert.Equal(0m, order.ShippingCost);
        Assert.Equal(9.60m, order.TaxAmount);
        Assert.Equal(129.58m, order.Total);
        Assert.Single(order.OrderLines);
        Assert.Equal("Portable Bluetooth Speaker", order.OrderLines.First().ProductName);
        Assert.Equal("buyer@example.com", order.ShipEmail); // address snapshotted
    }

    [Fact]
    public void PlaceOrder_ExpressShipping_AddsFlatRate()
    {
        var (checkout, cart) = CreateServices();
        var items = cart.AddItem(Array.Empty<SessionCartItem>(), 2, null, 1).Data!;

        var result = checkout.PlaceOrder(items, ValidAddress(), "Express", "CashOnDelivery", null);

        Assert.True(result.Success);
        Assert.Equal(12.99m, result.Data!.ShippingCost);
    }

    [Fact]
    public void PlaceOrder_DefaultsPaymentMethodToCashOnDelivery()
    {
        var (checkout, cart) = CreateServices();
        var items = cart.AddItem(Array.Empty<SessionCartItem>(), 2, null, 1).Data!;

        var result = checkout.PlaceOrder(items, ValidAddress(), "Standard", null, null);

        Assert.True(result.Success);
        Assert.Equal("CashOnDelivery", result.Data!.PaymentMethod);
    }

    [Fact]
    public void BuildSummary_TotalEqualsSubtotalPlusShippingPlusTax()
    {
        var (checkout, cart) = CreateServices();
        var items = cart.AddItem(Array.Empty<SessionCartItem>(), 1, 2, 1).Data!;

        var summary = checkout.BuildSummary(items, "Standard");

        Assert.Equal(summary.SubTotal + summary.ShippingCost + summary.TaxAmount, summary.Total);
        Assert.Equal("Standard", summary.ShippingMethod);
    }

    [Fact]
    public void GetOrder_UnknownId_ReturnsNull()
    {
        var (checkout, _) = CreateServices();
        Assert.Null(checkout.GetOrder(999));
    }

    [Fact]
    public void CanViewOrder_OwnerAndSessionOrder_Allowed()
    {
        var (checkout, cart) = CreateServices();
        var items = cart.AddItem(Array.Empty<SessionCartItem>(), 2, null, 1).Data!;
        var placed = checkout.PlaceOrder(items, ValidAddress(), "Standard", "CashOnDelivery", "user-9");
        var orderId = placed.Data!.Id;

        Assert.True(checkout.CanViewOrder(orderId, "user-9", null));
        Assert.True(checkout.CanViewOrder(orderId, null, orderId));
        Assert.False(checkout.CanViewOrder(orderId, "someone-else", null));
        Assert.False(checkout.CanViewOrder(orderId, null, null));
    }
}
