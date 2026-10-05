using LegacyEcom.Application.DTOs.Catalog;
using LegacyEcom.Application.Interfaces;

namespace LegacyEcom.Api.Endpoints;

/// <summary>
/// Catalog endpoints. Replaces <c>ProductController</c> (Index/Detail/Filter/
/// CategoryTree) and the catalog parts of <c>HomeController</c>.
/// </summary>
public static class CatalogEndpoints
{
    public static void MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Catalog");

        group.MapGet("/", (
                ICatalogService catalog,
                int? categoryId = null,
                string? q = null,
                int page = 1,
                int pageSize = 12) =>
            {
                var listing = catalog.GetListing(categoryId, q, page, pageSize);
                return Results.Ok(listing);
            })
            .WithName("GetProducts")
            .WithSummary("Paged product listing with optional category filter and search.")
            .Produces<ProductListDto>();

        group.MapGet("/{id:int}", (int id, ICatalogService catalog) =>
            {
                var detail = catalog.GetDetail(id);
                return detail is null
                    ? Results.NotFound(new { message = $"Product {id} not found." })
                    : Results.Ok(detail);
            })
            .WithName("GetProductById")
            .WithSummary("Product details with images, variants and related products.")
            .Produces<ProductDetailDto>()
            .Produces(404);

        group.MapGet("/featured", (int? take, ICatalogService catalog) =>
            {
                var count = take is >= 1 and <= 50 ? take.Value : 8;
                return Results.Ok(catalog.GetFeatured(count));
            })
            .WithName("GetFeaturedProducts")
            .WithSummary("Featured products for the storefront hero/grid (legacy HomeController).")
            .Produces<IReadOnlyList<ProductCardDto>>();

        var categories = app.MapGroup("/api/categories").WithTags("Catalog");

        categories.MapGet("/", (ICatalogService catalog) =>
                Results.Ok(catalog.GetCategories()))
            .WithName("GetCategories")
            .WithSummary("All active categories.")
            .Produces<IReadOnlyList<CategoryDto>>();

        categories.MapGet("/tree", (int? parentId, ICatalogService catalog) =>
                Results.Ok(catalog.GetCategoryTree(parentId)))
            .WithName("GetCategoryTree")
            .WithSummary("One level of the nested category tree (legacy ProductController.CategoryTree).")
            .Produces<IReadOnlyList<CategoryDto>>();
    }
}
