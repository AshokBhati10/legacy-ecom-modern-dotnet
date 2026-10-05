using LegacyEcom.Application.Interfaces;
using LegacyEcom.Application.Services;
using LegacyEcom.Domain.Interfaces;
using LegacyEcom.Infrastructure.Data;
using LegacyEcom.Infrastructure.Identity;
using LegacyEcom.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace LegacyEcom.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddEcommerceServices(this IServiceCollection services, IConfiguration config)
    {
        // --- EF Core + SQL Server (replaces EF6/EDMX) ---
        services.AddDbContext<EcommerceDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("EcommerceDb")));

        // --- Repositories + unit of work (scoped, like the EF6 DbContext) ---
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // --- Application services (scoped; no singleton state) ---
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IAccountService, AccountService>();

        // --- ASP.NET Core Identity (replaces Identity 2.0 + OWIN) ---
        services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                // Password policy mirrors the legacy PasswordValidator:
                // length 6, upper + lower + digit + non-alphanumeric.
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                // Email is the user name, and must be unique (legacy UserValidator).
                options.User.RequireUniqueEmail = true;

                // Lockout mirrors the legacy manager: 5 attempts -> 5 minute lockout.
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<EcommerceDbContext>()
            .AddDefaultTokenProviders();

        // Cookie auth mirrors the legacy OWIN cookie: 25-minute sliding expiration.
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(25);
            options.SlidingExpiration = true;
            // API behavior: 401/403 instead of redirecting to a login page.
            options.Events.OnRedirectToLogin = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        // --- Session (replaces InProc HttpContext.Session; 25-minute timeout) ---
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(25);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // --- Anti-forgery (replaces [ValidateAntiForgeryToken]) ---
        // The antiforgery system keeps its HttpOnly cookie token under its
        // default name; browser clients read the readable XSRF-TOKEN cookie
        // (set by GET /api/auth/xsrf-token) and send its value back in the
        // X-XSRF-TOKEN header on POST/PUT/DELETE.
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-XSRF-TOKEN";
        });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LegacyEcom Modern API",
                Version = "v1",
                Description = "Modernized e-commerce backend: ASP.NET Core Minimal APIs + EF Core + SQL Server. " +
                              "Port of the legacy ASP.NET MVC 5 / EF6 application."
            });

            // Cookie authentication (Identity) + anti-forgery header.
            options.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Cookie,
                Name = ".AspNetCore.Identity.Application",
                Description = "ASP.NET Core Identity cookie. Log in via POST /api/auth/login."
            });
            options.AddSecurityDefinition("xsrf", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "X-XSRF-TOKEN",
                Description = "Anti-forgery token. GET /api/auth/xsrf-token sets the XSRF-TOKEN cookie; send its value in this header on POST/PUT/DELETE."
            });
        });

        return services;
    }
}
