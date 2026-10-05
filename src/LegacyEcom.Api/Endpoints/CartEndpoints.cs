using System.Security.Claims;
using LegacyEcom.Api.Filters;
using LegacyEcom.Api.Infrastructure;
using LegacyEcom.Application.DTOs.Cart;
using LegacyEcom.Application.Interfaces;

namespace LegacyEcom.Api.Endpoints;

/// <summary>
/// Cart endpoints. Replaces <c>CartController</c>.
/// Anonymous shoppers: cart lives in server-side session (legacy InProc session).
/// Signed-in shoppers: cart is additionally persisted to the CartItems table on
/// every mutation and merged back into the session on login.
/// </summary>
public static class CartEndpoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cart").WithTags("Cart");

        group.MapGet("/", (HttpContext http, ICartService cart) =>
                Results.Ok(cart.GetCart(http.Session.GetCartItems())))
            .WithName("GetCart")
            .WithSummary("Current cart with line items and totals.")
            .Produces<CartDto>();

        group.MapGet("/mini", (HttpContext http, ICartService cart) =>
                Results.Ok(cart.GetMiniCart(http.Session.GetCartItems())))
            .WithName("GetMiniCart")
            .WithSummary("Cart item count and subtotal for the header (legacy MiniCart).")
            .Produces<MiniCartDto>();

        group.MapPost("/items", (HttpContext http, AddCartItemRequest request, ICartService cart) =>
            {
                var result = cart.AddItem(http.Session.GetCartItems(), request.ProductId, request.VariantId, request.Quantity);
                if (!result.Success)
                    return Results.BadRequest(new { message = result.Message });

                http.Session.SetCartItems(result.Data!);
                PersistForUser(http, cart);

                return Results.Ok(cart.GetCart(result.Data!));
            })
            .AddEndpointFilter<ValidationFilter>()
            .RequireAntiforgeryToken()
            .WithName("AddCartItem")
            .WithSummary("Add a product (optionally a variant) to the cart.")
            .Produces<CartDto>()
            .Produces(400);

        group.MapPut("/items", (HttpContext http, UpdateCartItemRequest request, ICartService cart) =>
            {
                var result = cart.UpdateItem(http.Session.GetCartItems(), request.ProductId, request.VariantId, request.Quantity);
                if (!result.Success)
                    return Results.NotFound(new { message = result.Message });

                http.Session.SetCartItems(result.Data!);
                PersistForUser(http, cart);

                return Results.Ok(cart.GetCart(result.Data!));
            })
            .AddEndpointFilter<ValidationFilter>()
            .RequireAntiforgeryToken()
            .WithName("UpdateCartItem")
            .WithSummary("Set a cart line's quantity (0 removes the line).")
            .Produces<CartDto>()
            .Produces(404);

        group.MapDelete("/items", (HttpContext http, int productId, int? variantId, ICartService cart) =>
            {
                var result = cart.RemoveItem(http.Session.GetCartItems(), productId, variantId);
                if (!result.Success)
                    return Results.NotFound(new { message = result.Message });

                http.Session.SetCartItems(result.Data!);
                PersistForUser(http, cart);

                return Results.Ok(cart.GetCart(result.Data!));
            })
            .RequireAntiforgeryToken()
            .WithName("RemoveCartItem")
            .WithSummary("Remove a line from the cart.")
            .Produces<CartDto>()
            .Produces(404);

        group.MapPost("/clear", (HttpContext http, ICartService cart) =>
            {
                http.Session.SetCartItems(cart.ClearCart());
                PersistForUser(http, cart);
                return Results.Ok(cart.GetCart(http.Session.GetCartItems()));
            })
            .RequireAntiforgeryToken()
            .WithName("ClearCart")
            .WithSummary("Empty the cart.")
            .Produces<CartDto>();
    }

    private static void PersistForUser(HttpContext http, ICartService cart)
    {
        var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
            cart.SaveForUser(userId, http.Session.GetCartItems());
    }
}
