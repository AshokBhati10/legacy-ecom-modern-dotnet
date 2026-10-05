using System.Security.Claims;
using LegacyEcom.Api.Filters;
using LegacyEcom.Api.Infrastructure;
using LegacyEcom.Application.DTOs.Cart;
using LegacyEcom.Application.DTOs.Checkout;
using LegacyEcom.Application.DTOs.Orders;
using LegacyEcom.Application.Interfaces;

namespace LegacyEcom.Api.Endpoints;

/// <summary>
/// Checkout endpoints. Replaces the <c>CheckoutController</c> wizard
/// (Address → Shipping → Payment → Confirmation). Wizard state lives in
/// server-side session, exactly like the legacy implementation; totals always
/// come from the services, never from the client.
/// </summary>
public static class CheckoutEndpoints
{
    public static void MapCheckoutEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/checkout").WithTags("Checkout");

        // ---- Step 1: Address -------------------------------------------------
        group.MapGet("/address", (HttpContext http, IAccountService account) =>
            {
                var stored = http.Session.GetCheckoutAddress<AddressRequest>();
                if (stored is not null) return Results.Ok(stored);

                // Prefill for signed-in shoppers (legacy CheckoutController.Address).
                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    var customer = account.GetCustomerByUserId(userId);
                    if (customer is not null)
                        return Results.Ok(new AddressRequest(
                            customer.Email, customer.FirstName, customer.LastName,
                            customer.Phone, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                }

                return Results.NoContent();
            })
            .WithName("GetCheckoutAddress")
            .WithSummary("Current checkout address, prefilled from the customer profile when signed in.")
            .Produces<AddressRequest>()
            .Produces(204);

        group.MapPost("/address", (HttpContext http, AddressRequest request, ICartService cart) =>
            {
                if (cart.GetCart(http.Session.GetCartItems()).Items.Count == 0)
                    return Results.BadRequest(new { message = "Your cart is empty." });

                http.Session.SetCheckoutAddress(request);
                return Results.NoContent();
            })
            .AddEndpointFilter<ValidationFilter>()
            .RequireAntiforgeryToken()
            .WithName("SetCheckoutAddress")
            .WithSummary("Step 1: validate and store the shipping address.")
            .Produces(204)
            .Produces(400);

        // ---- Step 2: Shipping ------------------------------------------------
        group.MapGet("/shipping-options", (HttpContext http, ICartService cart, ICheckoutService checkout) =>
            {
                var address = http.Session.GetCheckoutAddress<AddressRequest>();
                if (address is null)
                    return Results.Conflict(new { message = "Complete the address step first." });

                var items = http.Session.GetCartItems();
                if (cart.GetCart(items).Items.Count == 0)
                    return Results.BadRequest(new { message = "Your cart is empty." });

                var method = http.Session.GetCheckoutShippingMethod();
                return Results.Ok(new
                {
                    options = checkout.GetShippingOptions(items),
                    selectedMethod = method,
                    summary = checkout.BuildSummary(items, method)
                });
            })
            .WithName("GetShippingOptions")
            .WithSummary("Step 2: shipping options and order summary for the current cart.")
            .Produces(409)
            .Produces(400);

        group.MapPost("/shipping", (HttpContext http, SetShippingMethodRequest request, ICheckoutService checkout) =>
            {
                var address = http.Session.GetCheckoutAddress<AddressRequest>();
                if (address is null)
                    return Results.Conflict(new { message = "Complete the address step first." });

                var method = Application.Pricing.PriceCalculator.NormalizeShippingMethod(request.ShippingMethod);
                http.Session.SetCheckoutShippingMethod(method);
                return Results.Ok(new { shippingMethod = method });
            })
            .AddEndpointFilter<ValidationFilter>()
            .RequireAntiforgeryToken()
            .WithName("SetShippingMethod")
            .WithSummary("Step 2: choose the shipping method.")
            .Produces(409)
            .Produces(400);

        group.MapGet("/summary", (HttpContext http, string? shippingMethod, ICartService cart, ICheckoutService checkout) =>
            {
                var method = shippingMethod ?? http.Session.GetCheckoutShippingMethod();
                return Results.Ok(checkout.BuildSummary(http.Session.GetCartItems(), method));
            })
            .WithName("GetCheckoutSummary")
            .WithSummary("Order summary (items, shipping, tax, total) for a shipping method.")
            .Produces<CheckoutSummaryDto>();

        // ---- Step 3: Payment / place order ----------------------------------
        group.MapPost("/place-order", (HttpContext http, PlaceOrderRequest request,
                ICartService cart, ICheckoutService checkout) =>
            {
                var address = http.Session.GetCheckoutAddress<AddressRequest>();
                if (address is null)
                    return Results.Conflict(new { message = "Complete the address step first." });

                var shippingMethod = http.Session.GetCheckoutShippingMethod();
                if (string.IsNullOrWhiteSpace(shippingMethod))
                    return Results.Conflict(new { message = "Choose a shipping method first." });

                var items = http.Session.GetCartItems();
                if (cart.GetCart(items).Items.Count == 0)
                    return Results.BadRequest(new { message = "Your cart is empty." });

                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = checkout.PlaceOrder(items, address, shippingMethod, request.PaymentMethod, userId);
                if (!result.Success)
                    return Results.BadRequest(new { message = result.Message });

                // Order placed: clear cart and wizard state, remember the order id
                // in session so the confirmation cannot be guessed by URL.
                http.Session.ClearCart();
                if (!string.IsNullOrEmpty(userId))
                    cart.SaveForUser(userId, http.Session.GetCartItems());
                http.Session.ClearCheckoutState();
                http.Session.SetLastOrderId(result.Data!.Id);

                return Results.Created($"/api/orders/{result.Data.Id}",
                    new { orderId = result.Data.Id, orderNumber = result.Data.OrderNumber });
            })
            .AddEndpointFilter<ValidationFilter>()
            .RequireAntiforgeryToken()
            .WithName("PlaceOrder")
            .WithSummary("Step 3: place the order. Card fields are validated when paymentMethod is 'Card'.")
            .Produces(201)
            .Produces(400)
            .Produces(409);
    }
}
