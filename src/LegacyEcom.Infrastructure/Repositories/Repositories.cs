using LegacyEcom.Domain.Entities;
using LegacyEcom.Domain.Interfaces;
using LegacyEcom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyEcom.Infrastructure.Repositories;

/// <summary>
/// Database access boundary for products. All queries are LINQ-to-EF Core;
/// <c>Include</c> is used so listing/detail never N+1 (same as the legacy repo).
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly EcommerceDbContext _db;

    public ProductRepository(EcommerceDbContext db) => _db = db;

    public Product? GetById(int id) =>
        _db.Products
            .Include(x => x.Category)
            .Include(x => x.Images)
            .Include(x => x.Variants)
            .FirstOrDefault(x => x.Id == id && x.IsActive);

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Products
            .Include(x => x.Category)
            .Include(x => x.Images)
            .Include(x => x.Variants)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);

    public (IList<Product> Items, int TotalCount) GetListing(int? categoryId, string? q, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 12;

        var query = _db.Products
            .Include(x => x.Images)
            .Where(x => x.IsActive);

        if (categoryId.HasValue)
            query = query.Where(x => x.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(q))
        {
            // Explicit case-insensitive match: preserves the legacy SQL Server
            // CI-collation behavior on every provider (EF InMemory's Contains
            // is ordinal/case-sensitive, unlike SQL Server LIKE).
            var term = q.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(term) || x.Sku.ToLower().Contains(term));
        }

        var totalCount = query.Count();

        var items = query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }

    public IList<Product> GetFeatured(int take) =>
        _db.Products
            .Include(x => x.Images)
            .Where(x => x.IsActive && x.IsFeatured)
            .OrderByDescending(x => x.CreatedDate)
            .Take(take)
            .ToList();

    public IList<Product> GetRelated(int productId, int take)
    {
        var categoryId = _db.Products
            .Where(x => x.Id == productId)
            .Select(x => x.CategoryId)
            .FirstOrDefault();

        return _db.Products
            .Include(x => x.Images)
            .Where(x => x.IsActive && x.Id != productId && x.CategoryId == categoryId)
            .OrderByDescending(x => x.CreatedDate)
            .Take(take)
            .ToList();
    }
}

public class CategoryRepository : ICategoryRepository
{
    private readonly EcommerceDbContext _db;

    public CategoryRepository(EcommerceDbContext db) => _db = db;

    public IList<Category> GetActive() =>
        _db.Categories
            .Include(x => x.ChildCategories)
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ToList();

    public IList<Category> GetChildren(int? parentId) =>
        _db.Categories
            .Include(x => x.ChildCategories)
            .Where(x => x.IsActive && x.ParentCategoryId == parentId)
            .OrderBy(x => x.DisplayOrder)
            .ToList();

    public Category? GetById(int id) =>
        _db.Categories.FirstOrDefault(x => x.Id == id);
}

public class CustomerRepository : ICustomerRepository
{
    private readonly EcommerceDbContext _db;

    public CustomerRepository(EcommerceDbContext db) => _db = db;

    public Customer? GetByUserId(string userId) =>
        _db.Customers.FirstOrDefault(x => x.UserId == userId);

    public Customer? GetByEmail(string email) =>
        _db.Customers.FirstOrDefault(x => x.Email == email);

    public void Add(Customer customer) => _db.Customers.Add(customer);
}

public class CartRepository : ICartRepository
{
    private readonly EcommerceDbContext _db;

    public CartRepository(EcommerceDbContext db) => _db = db;

    public IList<CartItem> GetByUserId(string userId) =>
        _db.CartItems
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.DateCreated)
            .Select(x => new CartItem
            {
                Id = x.Id,
                UserId = x.UserId,
                ProductId = x.ProductId,
                VariantId = x.VariantId,
                Quantity = x.Quantity,
                DateCreated = x.DateCreated
            })
            .ToList();

    public void Save(string userId, IEnumerable<CartItem> items)
    {
        var existing = _db.CartItems.Where(x => x.UserId == userId).ToList();
        _db.CartItems.RemoveRange(existing);

        var now = DateTime.UtcNow;
        foreach (var item in items ?? Enumerable.Empty<CartItem>())
        {
            _db.CartItems.Add(new CartItem
            {
                UserId = userId,
                ProductId = item.ProductId,
                VariantId = item.VariantId,
                Quantity = item.Quantity,
                DateCreated = now
            });
        }
    }

    public void Clear(string userId)
    {
        var existing = _db.CartItems.Where(x => x.UserId == userId).ToList();
        _db.CartItems.RemoveRange(existing);
    }
}

public class OrderRepository : IOrderRepository
{
    private readonly EcommerceDbContext _db;

    public OrderRepository(EcommerceDbContext db) => _db = db;

    public Order? GetById(int id) =>
        _db.Orders
            .Include(x => x.OrderLines)
            .FirstOrDefault(x => x.Id == id);

    public Order? GetByOrderNumber(string orderNumber) =>
        _db.Orders
            .Include(x => x.OrderLines)
            .FirstOrDefault(x => x.OrderNumber == orderNumber);

    public IList<Order> GetByUserId(string userId) =>
        _db.Orders
            .Include(x => x.OrderLines)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.OrderDate)
            .ToList();

    public void Add(Order order) => _db.Orders.Add(order);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly EcommerceDbContext _db;

    public UnitOfWork(EcommerceDbContext db) => _db = db;

    public int SaveChanges() => _db.SaveChanges();

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}
