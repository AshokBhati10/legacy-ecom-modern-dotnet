using System.Net;
using LegacyEcom.Application.DTOs.Catalog;
using Xunit;

namespace LegacyEcom.Tests.Api;

public class CatalogApiTests : IClassFixture<EcommerceWebFactory>
{
    private readonly EcommerceWebFactory _factory;

    public CatalogApiTests(EcommerceWebFactory factory)
    {
        _factory = factory;
        _factory.SeedCatalog();
    }

    [Fact]
    public async Task GetProducts_ReturnsPagedListing()
    {
        var client = new ApiTestClient(_factory.CreateClient());

        var response = await client.GetAsync("/api/products?pageSize=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var listing = await ApiTestClient.ReadJsonAsync<ProductListDto>(response);
        Assert.Equal(2, listing.TotalCount);
        Assert.Single(listing.Products);
        Assert.Equal(2, listing.TotalPages);
    }

    [Fact]
    public async Task GetProducts_SearchFiltersByName()
    {
        var client = new ApiTestClient(_factory.CreateClient());

        var response = await client.GetAsync("/api/products?q=headphones");

        var listing = await ApiTestClient.ReadJsonAsync<ProductListDto>(response);
        Assert.Equal(1, listing.TotalCount);
        Assert.Equal("Wireless Headphones Pro", listing.Products[0].Name);
    }

    [Fact]
    public async Task GetProductById_ReturnsDetailWithVariants()
    {
        var client = new ApiTestClient(_factory.CreateClient());

        var response = await client.GetAsync("/api/products/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var detail = await ApiTestClient.ReadJsonAsync<ProductDetailDto>(response);
        Assert.Equal(119.99m, detail.EffectivePrice);
        Assert.Single(detail.Variants);
    }

    [Fact]
    public async Task GetProductById_UnknownId_Returns404()
    {
        var client = new ApiTestClient(_factory.CreateClient());

        var response = await client.GetAsync("/api/products/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCategories_ReturnsActiveCategories()
    {
        var client = new ApiTestClient(_factory.CreateClient());

        var response = await client.GetAsync("/api/categories");

        var categories = await ApiTestClient.ReadJsonAsync<List<CategoryDto>>(response);
        Assert.Single(categories);
        Assert.Equal("Audio", categories[0].Name);
    }
}
