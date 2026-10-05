using System.Net;
using System.Net.Http.Json;
using LegacyEcom.Application.DTOs.Cart;
using LegacyEcom.Application.DTOs.Orders;
using Xunit;

namespace LegacyEcom.Tests.Api;

/// <summary>
/// Full journey through the public API: register -> login -> cart ->
/// checkout wizard -> order -> history, plus the key rejection cases.
/// </summary>
public class AuthCartCheckoutApiTests : IClassFixture<EcommerceWebFactory>
{
    private readonly EcommerceWebFactory _factory;

    public AuthCartCheckoutApiTests(EcommerceWebFactory factory)
    {
        _factory = factory;
        _factory.SeedCatalog();
    }

    private static string UniqueEmail() => $"api-test-{Guid.NewGuid():N}@example.com";

    private async Task<ApiTestClient> RegisterAndLoginAsync(string? email = null)
    {
        email ??= UniqueEmail();
        var client = new ApiTestClient(_factory.CreateClient());

        var register = await client.PostAsync("/api/auth/register", new
        {
            firstName = "Api",
            lastName = "Tester",
            email,
            password = "Str0ng!Pass",
            confirmPassword = "Str0ng!Pass"
        });
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        return client; // register also signs the user in
    }

    [Fact]
    public async Task Register_WeakPassword_Returns400()
    {
        var client = new ApiTestClient(_factory.CreateClient());

        var response = await client.PostAsync("/api/auth/register", new
        {
            firstName = "Api",
            lastName = "Tester",
            email = UniqueEmail(),
            password = "weak",
            confirmPassword = "weak"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409()
    {
        var email = UniqueEmail();
        await RegisterAndLoginAsync(email);
        var second = new ApiTestClient(_factory.CreateClient());

        var response = await second.PostAsync("/api/auth/register", new
        {
            firstName = "Api",
            lastName = "Tester",
            email,
            password = "Str0ng!Pass",
            confirmPassword = "Str0ng!Pass"
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var email = UniqueEmail();
        await RegisterAndLoginAsync(email);
        var client = new ApiTestClient(_factory.CreateClient());

        var response = await client.PostAsync("/api/auth/login", new
        {
            email,
            password = "Wr0ng!Pass"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CartMutations_WithoutXsrfToken_Returns400()
    {
        var client = new ApiTestClient(_factory.CreateClient());

        // Raw POST without going through the XSRF flow.
        using var raw = await _factory.CreateClient().PostAsJsonAsync(
            "/api/cart/items", new { productId = 1, quantity = 1 });

        Assert.Equal(HttpStatusCode.BadRequest, raw.StatusCode);
    }

    [Fact]
    public async Task FullCheckoutJourney_CreatesOrderAndHistory()
    {
        var client = await RegisterAndLoginAsync();

        // Cart
        var add = await client.PostAsync("/api/cart/items", new { productId = 1, variantId = 1, quantity = 2 });
        Assert.Equal(HttpStatusCode.OK, add.StatusCode);
        var cart = await ApiTestClient.ReadJsonAsync<CartDto>(add);
        Assert.Equal(2, cart.ItemCount);
        Assert.Equal(239.98m, cart.SubTotal); // 2 x 119.99 (sale price)

        // Checkout wizard: address -> shipping -> place order
        var badAddress = await client.PostAsync("/api/checkout/address", new { email = "nope" });
        Assert.Equal(HttpStatusCode.BadRequest, badAddress.StatusCode);

        var shippingFirst = await client.GetAsync("/api/checkout/shipping-options");
        Assert.Equal(HttpStatusCode.Conflict, shippingFirst.StatusCode);

        var address = await client.PostAsync("/api/checkout/address", new
        {
            email = "buyer@example.com",
            firstName = "Api",
            lastName = "Tester",
            street = "1 Market St",
            city = "Springfield",
            state = "IL",
            postalCode = "62701",
            country = "USA"
        });
        Assert.Equal(HttpStatusCode.NoContent, address.StatusCode);

        var options = await client.GetAsync("/api/checkout/shipping-options");
        Assert.Equal(HttpStatusCode.OK, options.StatusCode);

        var setShipping = await client.PostAsync("/api/checkout/shipping", new { shippingMethod = "Standard" });
        Assert.Equal(HttpStatusCode.OK, setShipping.StatusCode);

        var place = await client.PostAsync("/api/checkout/place-order", new { paymentMethod = "CashOnDelivery" });
        Assert.Equal(HttpStatusCode.Created, place.StatusCode);
        Assert.NotNull(place.Headers.Location);

        var orderId = int.Parse(place.Headers.Location!.ToString().Split('/').Last());

        // Order detail + history
        var detail = await client.GetAsync($"/api/orders/{orderId}");
        Assert.Equal(HttpStatusCode.OK, detail.StatusCode);
        var order = await ApiTestClient.ReadJsonAsync<OrderDto>(detail);
        Assert.Equal("Placed", order.Status);
        Assert.StartsWith("LE-", order.OrderNumber);
        Assert.Equal(239.98m, order.SubTotal);

        var history = await client.GetAsync("/api/orders");
        var orders = await ApiTestClient.ReadJsonAsync<List<OrderSummaryDto>>(history);
        Assert.Single(orders);
        Assert.Equal(order.OrderNumber, orders[0].OrderNumber);

        // Cart was cleared by the order placement.
        var empty = await client.GetAsync("/api/cart");
        var emptyCart = await ApiTestClient.ReadJsonAsync<CartDto>(empty);
        Assert.Equal(0, emptyCart.ItemCount);
    }

    [Fact]
    public async Task OrderHistory_Anonymous_Returns401()
    {
        var client = new ApiTestClient(_factory.CreateClient());

        var response = await client.GetAsync("/api/orders");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_ClearsAuthCookie()
    {
        var client = await RegisterAndLoginAsync();

        var logout = await client.PostAsync("/api/auth/logout", null);
        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);

        var me = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, me.StatusCode);
    }
}
