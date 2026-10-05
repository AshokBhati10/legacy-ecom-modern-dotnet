using LegacyEcom.Application.DTOs.Cart;
using LegacyEcom.Application.Interfaces;
using LegacyEcom.Application.Pricing;
using LegacyEcom.Domain.Entities;
using LegacyEcom.Domain.Interfaces;

namespace LegacyEcom.Application.Services;

public class CartService : ICartService
{
    private readonly IProductRepository _products;
    private readonly ICartRepository _carts;
    private readonly IUnitOfWork _unitOfWork;

    private const int MaxQuantityPerLine = 99;

    public CartService(IProductRepository products, ICartRepository carts, IUnitOfWork unitOfWork)
    {
        _products = products;
        _carts = carts;
        _unitOfWork = unitOfWork;
    }

    public CartDto GetCart(IEnumerable<SessionCartItem> items)
    {
        var lines = new List<CartLineDto>();
        foreach (var item in items ?? Enumerable.Empty<SessionCartItem>())
        {
            var line = BuildLine(item);
            if (line is not null) lines.Add(line);
        }

        return new CartDto(
            lines,
            lines.Sum(x => x.Quantity),
            PriceCalculator.CalculateSubTotal(lines));
    }

    public MiniCartDto GetMiniCart(IEnumerable<SessionCartItem> items)
    {
        var cart = GetCart(items);
        return new MiniCartDto(cart.ItemCount, cart.SubTotal);
    }

    public ServiceResult<IReadOnlyList<SessionCartItem>> AddItem(
        IEnumerable<SessionCartItem> items, int productId, int? variantId, int quantity)
    {
        if (quantity < 1) quantity = 1;
        if (quantity > MaxQuantityPerLine) quantity = MaxQuantityPerLine;

        var product = _products.GetById(productId);
        if (product is null)
            return ServiceResult<IReadOnlyList<SessionCartItem>>.Fail("Product not found.");

        if (variantId.HasValue)
        {
            var variant = product.Variants.FirstOrDefault(v => v.Id == variantId.Value);
            if (variant is null)
                return ServiceResult<IReadOnlyList<SessionCartItem>>.Fail("Product variant not found.");
        }

        var list = (items ?? Enumerable.Empty<SessionCartItem>())
            .Select(x => new SessionCartItem(x.ProductId, x.VariantId, x.Quantity))
            .ToList();

        var existing = list.FirstOrDefault(x => x.ProductId == productId && x.VariantId == variantId);
        if (existing is not null)
        {
            list.Remove(existing);
            list.Add(existing with { Quantity = Math.Min(existing.Quantity + quantity, MaxQuantityPerLine) });
        }
        else
        {
            list.Add(new SessionCartItem(productId, variantId, quantity));
        }

        return ServiceResult<IReadOnlyList<SessionCartItem>>.Ok(list, "Added to cart.");
    }

    public ServiceResult<IReadOnlyList<SessionCartItem>> UpdateItem(
        IEnumerable<SessionCartItem> items, int productId, int? variantId, int quantity)
    {
        var list = (items ?? Enumerable.Empty<SessionCartItem>())
            .Select(x => new SessionCartItem(x.ProductId, x.VariantId, x.Quantity))
            .ToList();

        var existing = list.FirstOrDefault(x => x.ProductId == productId && x.VariantId == variantId);
        if (existing is null)
            return ServiceResult<IReadOnlyList<SessionCartItem>>.Fail("Item not found in cart.");

        list.Remove(existing);
        if (quantity > 0)
            list.Add(existing with { Quantity = Math.Min(quantity, MaxQuantityPerLine) });

        return ServiceResult<IReadOnlyList<SessionCartItem>>.Ok(list, "Cart updated.");
    }

    public ServiceResult<IReadOnlyList<SessionCartItem>> RemoveItem(
        IEnumerable<SessionCartItem> items, int productId, int? variantId) =>
        UpdateItem(items, productId, variantId, 0);

    public IReadOnlyList<SessionCartItem> ClearCart() =>
        new List<SessionCartItem>();

    public void SaveForUser(string userId, IEnumerable<SessionCartItem> items)
    {
        if (string.IsNullOrWhiteSpace(userId)) return;
        _carts.Save(userId, (items ?? Enumerable.Empty<SessionCartItem>()).Select(x => new CartItem
        {
            ProductId = x.ProductId,
            VariantId = x.VariantId,
            Quantity = x.Quantity
        }));
        _unitOfWork.SaveChanges();
    }

    public IList<SessionCartItem> LoadForUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return new List<SessionCartItem>();
        return _carts.GetByUserId(userId)
            .Select(x => new SessionCartItem(x.ProductId, x.VariantId, x.Quantity))
            .ToList();
    }

    private CartLineDto? BuildLine(SessionCartItem item)
    {
        var product = _products.GetById(item.ProductId);
        if (product is null) return null;

        string? variantName = null;
        var sku = product.Sku;
        var unitPrice = product.EffectivePrice;
        if (item.VariantId.HasValue)
        {
            var variant = product.Variants.FirstOrDefault(v => v.Id == item.VariantId.Value);
            if (variant is null) return null;
            variantName = variant.Name;
            sku = variant.Sku ?? product.Sku;
            unitPrice += variant.PriceAdjustment;
        }

        return new CartLineDto(
            product.Id,
            item.VariantId,
            product.Name,
            variantName,
            sku,
            product.ThumbnailUrl,
            unitPrice,
            item.Quantity,
            PriceCalculator.CalculateLineTotal(unitPrice, item.Quantity));
    }
}
