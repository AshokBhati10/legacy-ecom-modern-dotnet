using System.ComponentModel.DataAnnotations;

namespace LegacyEcom.Application.DTOs.Cart;

/// <summary>
/// One line in the shopper's cart as carried in session / persisted rows.
/// </summary>
public record SessionCartItem(int ProductId, int? VariantId, int Quantity);

// Note: validation attributes on positional records need the `property:`
// target, otherwise they land on the constructor parameter and
// Validator.TryValidateObject never sees them.
public record AddCartItemRequest(
    [property: Range(1, int.MaxValue)] int ProductId,
    int? VariantId,
    [property: Range(1, 99)] int Quantity = 1);

public record UpdateCartItemRequest(
    [property: Range(1, int.MaxValue)] int ProductId,
    int? VariantId,
    [property: Range(0, 99)] int Quantity);

public record CartLineDto(
    int ProductId,
    int? VariantId,
    string ProductName,
    string? VariantName,
    string? Sku,
    string? ThumbnailUrl,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

public record CartDto(
    IReadOnlyList<CartLineDto> Items,
    int ItemCount,
    decimal SubTotal);

public record MiniCartDto(int ItemCount, decimal SubTotal);
