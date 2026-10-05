using LegacyEcom.Domain.Entities;

namespace LegacyEcom.Domain.Interfaces;

public interface IProductRepository
{
    Product? GetById(int id);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    (IList<Product> Items, int TotalCount) GetListing(int? categoryId, string? q, int page, int pageSize);
    IList<Product> GetFeatured(int take);
    IList<Product> GetRelated(int productId, int take);
}

public interface ICategoryRepository
{
    IList<Category> GetActive();
    IList<Category> GetChildren(int? parentId);
    Category? GetById(int id);
}

public interface ICustomerRepository
{
    Customer? GetByUserId(string userId);
    Customer? GetByEmail(string email);
    void Add(Customer customer);
}

public interface ICartRepository
{
    IList<CartItem> GetByUserId(string userId);
    void Save(string userId, IEnumerable<CartItem> items);
    void Clear(string userId);
}

public interface IOrderRepository
{
    Order? GetById(int id);
    Order? GetByOrderNumber(string orderNumber);
    IList<Order> GetByUserId(string userId);
    void Add(Order order);
}

public interface IUnitOfWork
{
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
