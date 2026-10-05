namespace LegacyEcom.Application.DTOs.Orders;

public record OrderLineDto(
    int Id,
    int ProductId,
    int? VariantId,
    string ProductName,
    string? VariantName,
    string? Sku,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

public record OrderDto(
    int Id,
    string OrderNumber,
    DateTime OrderDate,
    string Status,
    decimal SubTotal,
    decimal ShippingCost,
    decimal TaxAmount,
    decimal Total,
    string ShippingMethod,
    string PaymentMethod,
    string ShipFirstName,
    string ShipLastName,
    string ShipEmail,
    string? ShipPhone,
    string ShipStreet,
    string ShipCity,
    string ShipState,
    string ShipPostalCode,
    string ShipCountry,
    IReadOnlyList<OrderLineDto> Lines);

public record OrderSummaryDto(
    int OrderId,
    string OrderNumber,
    DateTime OrderDate,
    string Status,
    int ItemCount,
    decimal Total);
