namespace LegacyEcom.Application.DTOs.Catalog;

public record ProductCardDto(
    int Id,
    string Name,
    string Slug,
    decimal Price,
    decimal? SalePrice,
    decimal EffectivePrice,
    string? ThumbnailUrl);

public record ProductImageDto(
    int Id,
    string Url,
    string? AltText,
    int DisplayOrder,
    bool IsMain);

public record ProductVariantDto(
    int Id,
    string Name,
    string? Sku,
    decimal PriceAdjustment,
    int StockQuantity,
    bool IsActive);

public record CategoryDto(
    int Id,
    string Name,
    string Slug,
    string? Description,
    bool HasChildren);

public record ProductDetailDto(
    int Id,
    string Sku,
    string Name,
    string Slug,
    string? ShortDescription,
    string? Description,
    decimal Price,
    decimal? SalePrice,
    decimal EffectivePrice,
    int CategoryId,
    string? CategoryName,
    string? ThumbnailUrl,
    bool IsFeatured,
    int StockQuantity,
    IReadOnlyList<ProductImageDto> Images,
    IReadOnlyList<ProductVariantDto> Variants,
    IReadOnlyList<ProductCardDto> RelatedProducts);

public record PagedResultDto<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

public record ProductListDto(
    IReadOnlyList<ProductCardDto> Products,
    IReadOnlyList<CategoryDto> Categories,
    int? SelectedCategoryId,
    string? SearchQuery,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
