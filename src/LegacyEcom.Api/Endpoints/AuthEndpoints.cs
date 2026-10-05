using System.Security.Claims;
using LegacyEcom.Api.Filters;
using LegacyEcom.Api.Infrastructure;
using LegacyEcom.Application.DTOs.Auth;
using LegacyEcom.Application.DTOs.Cart;
using LegacyEcom.Application.Interfaces;
using LegacyEcom.Infrastructure.Identity;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;

namespace LegacyEcom.Api.Endpoints;

/// <summary>
/// Authentication endpoints. Replaces <c>AccountController</c> (Login/Register/
/// LogOff/Orders) and the OWIN cookie middleware: ASP.NET Core Identity with
/// cookie authentication, the same password policy and lockout rules.
/// </summary>
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        // Issues the XSRF-TOKEN cookie consumed by the X-XSRF-TOKEN header on
        // POST/PUT/DELETE (replaces @Html.AntiForgeryToken()).
        group.MapGet("/xsrf-token", (HttpContext http, IAntiforgery antiforgery) =>
            {
                var tokens = antiforgery.GetAndStoreTokens(http);
                http.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
                    new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.Lax });
                return Results.NoContent();
            })
            .WithName("GetXsrfToken")
            .WithSummary("Issue an anti-forgery token cookie for state-changing requests.")
            .Produces(204);

        group.MapPost("/register", async (
                HttpContext http,
                RegisterRequest request,
                UserManager<AppUser> userManager,
                SignInManager<AppUser> signInManager,
                IAccountService account,
                ICartService cart) =>
            {
                var user = new AppUser { UserName = request.Email, Email = request.Email };
                var result = await userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    // Duplicate email -> 409, like a unique-constraint conflict.
                    if (result.Errors.Any(e => e.Code.Contains("Duplicate")))
                        return Results.Conflict(new
                        {
                            message = "An account with this email already exists.",
                            errors = result.Errors.Select(e => e.Description)
                        });

                    return Results.BadRequest(new
                    {
                        message = "Registration failed.",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                account.EnsureCustomerForUser(user.Id, request.Email, request.FirstName, request.LastName);
                await signInManager.SignInAsync(user, isPersistent: false);
                MergePersistedCart(http, cart, user.Id);

                return Results.Created("/api/auth/me", account.ToUserDto(user.Id, user.Email!));
            })
            .AddEndpointFilter<ValidationFilter>()
            .RequireAntiforgeryToken()
            .WithName("Register")
            .WithSummary("Register a new account (email login, Identity password policy).")
            .Produces<UserDto>(201)
            .Produces(400)
            .Produces(409);

        group.MapPost("/login", async (
                HttpContext http,
                LoginRequest request,
                SignInManager<AppUser> signInManager,
                UserManager<AppUser> userManager,
                IAccountService account,
                ICartService cart) =>
            {
                // shouldLockout: true, like the legacy PasswordSignInAsync call.
                var result = await signInManager.PasswordSignInAsync(
                    request.Email, request.Password, request.RememberMe, lockoutOnFailure: true);

                if (result.IsLockedOut)
                    return Results.Problem(
                        title: "Account locked out.",
                        detail: "Too many failed login attempts. Try again in a few minutes.",
                        statusCode: StatusCodes.Status423Locked);

                if (!result.Succeeded)
                    return Results.Problem(
                        title: "Invalid login attempt.",
                        detail: "The email or password is incorrect.",
                        statusCode: StatusCodes.Status401Unauthorized);

                var user = await userManager.FindByEmailAsync(request.Email);
                MergePersistedCart(http, cart, user?.Id);

                return Results.Ok(account.ToUserDto(user!.Id, user.Email!));
            })
            .AddEndpointFilter<ValidationFilter>()
            .RequireAntiforgeryToken()
            .WithName("Login")
            .WithSummary("Sign in with email + password. Locks out after 5 failed attempts.")
            .Produces<UserDto>()
            .Produces(401)
            .Produces(423);

        group.MapPost("/logout", async (
                HttpContext http,
                SignInManager<AppUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.NoContent();
            })
            .RequireAntiforgeryToken()
            .WithName("Logout")
            .WithSummary("Sign out (clears the auth cookie).")
            .Produces(204);

        group.MapGet("/me", (HttpContext http, IAccountService account) =>
            {
                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = http.User.FindFirstValue(ClaimTypes.Email)
                            ?? http.User.Identity?.Name;
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                    return Results.Unauthorized();

                return Results.Ok(account.ToUserDto(userId, email));
            })
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .WithSummary("Current signed-in user.")
            .Produces<UserDto>()
            .Produces(401);
    }

    /// <summary>
    /// Merges DB-persisted cart rows into the session cart on sign-in
    /// (legacy <c>AccountController.MergePersistedCart</c>).
    /// </summary>
    private static void MergePersistedCart(HttpContext http, ICartService cart, string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return;

        var merged = http.Session.GetCartItems().ToList();
        foreach (var saved in cart.LoadForUser(userId))
        {
            var existing = merged.FirstOrDefault(x =>
                x.ProductId == saved.ProductId && x.VariantId == saved.VariantId);
            if (existing is not null)
            {
                merged.Remove(existing);
                merged.Add(existing with { Quantity = existing.Quantity + saved.Quantity });
            }
            else
            {
                merged.Add(new SessionCartItem(saved.ProductId, saved.VariantId, saved.Quantity));
            }
        }
        http.Session.SetCartItems(merged);
        cart.SaveForUser(userId, merged);
    }
}
