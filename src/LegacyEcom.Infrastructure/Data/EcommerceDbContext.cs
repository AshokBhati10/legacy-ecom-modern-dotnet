using LegacyEcom.Domain.Entities;
using LegacyEcom.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LegacyEcom.Infrastructure.Data;

/// <summary>
/// EF Core DbContext for the modernized store. Replaces the EF6 Database-First
/// EDMX model: entities are plain POCOs in <c>LegacyEcom.Domain</c> and the
/// mapping lives in <c>Data/Configurations</c> (code-first, no EDMX).
/// Table and column names intentionally match the legacy <c>LegacyEcommerceDb</c>
/// schema so the data model stays recognizable; the new database is
/// <c>LegacyEcommerceModernDb</c> and is created by EF Core migrations.
/// </summary>
public class EcommerceDbContext : IdentityDbContext<AppUser>
{
    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcommerceDbContext).Assembly);
    }
}
