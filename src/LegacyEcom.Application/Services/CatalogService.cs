using LegacyEcom.Application.DTOs.Catalog;
using LegacyEcom.Application.Interfaces;
using LegacyEcom.Domain.Entities;
using LegacyEcom.Domain.Interfaces;

namespace LegacyEcom.Application.Services;

public class CatalogService : ICatalogService
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;

    public CatalogService(IProductRepository products, ICategoryRepository categories)
    {
        _products = products;
        _categories = categories;
    }

    public ProductListDto GetListing(int? categoryId, string? q, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 12;

        var (items, totalCount) = _products.GetListing(categoryId, q, page, pageSize);

        return new ProductListDto(
            items.Select(ToCard).ToList(),
            _categories.GetActive().Select(ToCategory).ToList(),
            categoryId,
            q,
            page,
            pageSize,
            totalCount,
            (int)Math.Ceiling((double)totalCount / pageSize));
    }

    public ProductDetailDto? GetDetail(int productId)
    {
        var p = _products.GetById(productId);
        if (p is null) return null;

        return new ProductDetailDto(
            p.Id, p.Sku, p.Name, p.Slug,
            p.ShortDescription, p.Description,
            p.Price, p.SalePrice, p.EffectivePrice,
            p.CategoryId, p.Category?.Name,
            p.ThumbnailUrl, p.IsFeatured, p.StockQuantity,
            p.Images.OrderBy(i => i.DisplayOrder).Select(i =>
                new ProductImageDto(i.Id, i.Url, i.AltText, i.DisplayOrder, i.IsMain)).ToList(),
            p.Variants.Where(v => v.IsActive).Select(v =>
                new ProductVariantDto(v.Id, v.Name, v.Sku, v.PriceAdjustment, v.StockQuantity, v.IsActive)).ToList(),
            _products.GetRelated(productId, 4).Select(ToCard).ToList());
    }

    public IReadOnlyList<ProductCardDto> GetFeatured(int take) =>
        _products.GetFeatured(take).Select(ToCard).ToList();

    public IReadOnlyList<CategoryDto> GetCategories() =>
        _categories.GetActive().Select(ToCategory).ToList();

    public IReadOnlyList<CategoryDto> GetCategoryTree(int? parentId) =>
        _categories.GetChildren(parentId).Select(ToCategory).ToList();

    private static CategoryDto ToCategory(Category c) => new(
        c.Id, c.Name, c.Slug, c.Description,
        c.ChildCategories.Any(x => x.IsActive));

    private static ProductCardDto ToCard(Product p) => new(
        p.Id, p.Name, p.Slug, p.Price, p.SalePrice, p.EffectivePrice, p.ThumbnailUrl);
}
