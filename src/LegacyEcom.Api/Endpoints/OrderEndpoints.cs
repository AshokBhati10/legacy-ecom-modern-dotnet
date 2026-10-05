using System.Security.Claims;
using LegacyEcom.Api.Infrastructure;
using LegacyEcom.Application.DTOs.Orders;
using LegacyEcom.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LegacyEcom.Api.Endpoints;

/// <summary>
/// Order endpoints. Replaces <c>AccountController.Orders</c> (order history)
/// and <c>CheckoutController.Confirmation</c> (order confirmation).
/// </summary>
public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapGet("/", (HttpContext http, IAccountService account) =>
            {
                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                return Results.Ok(account.GetOrderHistory(userId));
            })
            .RequireAuthorization()
            .WithName("GetOrderHistory")
            .WithSummary("Order history for the signed-in shopper (legacy Account/Orders).")
            .Produces<IReadOnlyList<OrderSummaryDto>>()
            .Produces(401);

        group.MapGet("/{id:int}", (int id, HttpContext http, ICheckoutService checkout) =>
            {
                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var lastOrderId = http.Session.GetLastOrderId();

                // Do not leak order existence: non-owners get 404, like the
                // legacy Confirmation action's HttpNotFound.
                if (!checkout.CanViewOrder(id, userId, lastOrderId))
                    return Results.NotFound(new { message = $"Order {id} not found." });

                var order = checkout.GetOrder(id);
                return order is null
                    ? Results.NotFound(new { message = $"Order {id} not found." })
                    : Results.Ok(order);
            })
            .WithName("GetOrderById")
            .WithSummary("Order details. Signed-in owners or the just-placed session order (legacy Confirmation).")
            .Produces<OrderDto>()
            .Produces(404);
    }
}
