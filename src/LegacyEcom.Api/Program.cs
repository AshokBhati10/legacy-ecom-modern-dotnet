using LegacyEcom.Api.Endpoints;
using LegacyEcom.Api.Extensions;
using LegacyEcom.Api.Middleware;
using LegacyEcom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEcommerceServices(builder.Configuration);
builder.Services.AddSwagger();

var app = builder.Build();

// --- Database: apply EF Core migrations, then seed (dev) -------------------
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var db = scope.ServiceProvider.GetRequiredService<EcommerceDbContext>();
    try
    {
        // Relational providers (SQL Server) get real migrations; anything else
        // (e.g. the InMemory provider used by tests) just gets a created schema.
        if (db.Database.IsRelational())
            await db.Database.MigrateAsync();
        else
            await db.Database.EnsureCreatedAsync();
        logger.LogInformation("Database ready.");

        if (app.Configuration.GetValue<bool>("SeedData:Enabled"))
            await DbSeeder.SeedAsync(db, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed.");
        throw;
    }
}

// --- Pipeline ---------------------------------------------------------------
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "LegacyEcom Modern API v1");
        options.DocumentTitle = "LegacyEcom Modern API";
    });
}

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// --- Minimal API endpoints (feature groups, not controllers) ----------------
app.MapCatalogEndpoints();
app.MapCartEndpoints();
app.MapCheckoutEndpoints();
app.MapOrderEndpoints();
app.MapAuthEndpoints();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "healthy",
    time = DateTime.UtcNow
}))
.WithTags("Health")
.WithSummary("Liveness probe.")
.Produces(200);

app.Run();

// Exposed for WebApplicationFactory integration tests.
public partial class Program { }
