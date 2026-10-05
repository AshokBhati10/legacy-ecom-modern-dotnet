using LegacyEcom.Application.DTOs.Auth;
using LegacyEcom.Application.DTOs.Cart;
using LegacyEcom.Application.DTOs.Catalog;
using LegacyEcom.Application.DTOs.Checkout;
using LegacyEcom.Application.DTOs.Orders;
using LegacyEcom.Domain.Entities;

namespace LegacyEcom.Application.Interfaces;

public interface ICatalogService
{
    ProductListDto GetListing(int? categoryId, string? q, int page, int pageSize);
    ProductDetailDto? GetDetail(int productId);
    IReadOnlyList<ProductCardDto> GetFeatured(int take);
    IReadOnlyList<CategoryDto> GetCategories();
    IReadOnlyList<CategoryDto> GetCategoryTree(int? parentId);
}

public interface ICartService
{
    CartDto GetCart(IEnumerable<SessionCartItem> items);
    MiniCartDto GetMiniCart(IEnumerable<SessionCartItem> items);
    ServiceResult<IReadOnlyList<SessionCartItem>> AddItem(IEnumerable<SessionCartItem> items, int productId, int? variantId, int quantity);
    ServiceResult<IReadOnlyList<SessionCartItem>> UpdateItem(IEnumerable<SessionCartItem> items, int productId, int? variantId, int quantity);
    ServiceResult<IReadOnlyList<SessionCartItem>> RemoveItem(IEnumerable<SessionCartItem> items, int productId, int? variantId);
    IReadOnlyList<SessionCartItem> ClearCart();
    void SaveForUser(string userId, IEnumerable<SessionCartItem> items);
    IList<SessionCartItem> LoadForUser(string userId);
}

public interface ICheckoutService
{
    CheckoutSummaryDto BuildSummary(IEnumerable<SessionCartItem> items, string? shippingMethod);
    IReadOnlyList<ShippingOptionDto> GetShippingOptions(IEnumerable<SessionCartItem> items);
    ServiceResult<Order> PlaceOrder(
        IEnumerable<SessionCartItem> items,
        AddressRequest address,
        string? shippingMethod,
        string? paymentMethod,
        string? userId);
    OrderDto? GetOrder(int orderId);

    /// <summary>
    /// A shopper may view an order if they own it (signed in) or if it is the
    /// order just placed in this session (guest confirmation flow).
    /// </summary>
    bool CanViewOrder(int orderId, string? userId, int? sessionLastOrderId);
}

public interface IAccountService
{
    IReadOnlyList<OrderSummaryDto> GetOrderHistory(string userId);
    Customer EnsureCustomerForUser(string userId, string email, string firstName, string lastName);
    Customer? GetCustomerByUserId(string userId);
    UserDto? ToUserDto(string userId, string email);
}

/// <summary>
/// Operation result. <see cref="Success"/> false means the caller should map
/// <see cref="Message"/> to a 4xx response; never throws for business failures.
/// </summary>
public sealed class ServiceResult<T>
{
    public bool Success { get; }
    public string? Message { get; }
    public T? Data { get; }

    private ServiceResult(bool success, T? data, string? message)
    {
        Success = success;
        Data = data;
        Message = message;
    }

    public static ServiceResult<T> Ok(T data, string? message = null) => new(true, data, message);
    public static ServiceResult<T> Fail(string message) => new(false, default, message);
}
