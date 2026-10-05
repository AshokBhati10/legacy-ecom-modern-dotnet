using System.ComponentModel.DataAnnotations;

namespace LegacyEcom.Application.DTOs.Checkout;

// Note: validation attributes on positional records need the `property:`
// target, otherwise they land on the constructor parameter and
// Validator.TryValidateObject never sees them.
public record AddressRequest(
    [property: Required, EmailAddress, MaxLength(256)] string Email,
    [property: Required, MaxLength(100)] string FirstName,
    [property: Required, MaxLength(100)] string LastName,
    [property: MaxLength(30)] string? Phone,
    [property: Required, MaxLength(250)] string Street,
    [property: Required, MaxLength(100)] string City,
    [property: Required, MaxLength(100)] string State,
    [property: Required, MaxLength(20)] string PostalCode,
    [property: Required, MaxLength(100)] string Country);

public record SetShippingMethodRequest(
    [property: Required(ErrorMessage = "Please choose a shipping method.")] string ShippingMethod);

public record ShippingOptionDto(
    string Code,
    string Name,
    string Description,
    decimal Cost);

public record CheckoutSummaryDto(
    IReadOnlyList<Cart.CartLineDto> Items,
    int ItemCount,
    decimal SubTotal,
    string ShippingMethod,
    decimal ShippingCost,
    decimal TaxAmount,
    decimal Total);

/// <summary>
/// Card fields are validated only when <see cref="PaymentMethod"/> is "Card"
/// (same rule as the legacy <c>RequiredIfCardAttribute</c>).
/// </summary>
public record PlaceOrderRequest(
    [property: Required] string PaymentMethod,
    [property: MaxLength(100)] string? CardholderName,
    [property: MaxLength(19)] string? CardNumber,
    [property: Range(1, 12)] int? ExpiryMonth,
    [property: Range(2026, 2040)] int? ExpiryYear,
    [property: MinLength(3), MaxLength(4)] string? Cvv)
    : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var isCard = string.Equals(PaymentMethod, "Card", StringComparison.OrdinalIgnoreCase);
        if (!isCard) yield break;

        if (string.IsNullOrWhiteSpace(CardholderName))
            yield return new ValidationResult("Cardholder name is required for card payments.", new[] { nameof(CardholderName) });
        if (string.IsNullOrWhiteSpace(CardNumber))
            yield return new ValidationResult("Card number is required for card payments.", new[] { nameof(CardNumber) });
        if (!ExpiryMonth.HasValue)
            yield return new ValidationResult("Expiry month is required for card payments.", new[] { nameof(ExpiryMonth) });
        if (!ExpiryYear.HasValue)
            yield return new ValidationResult("Expiry year is required for card payments.", new[] { nameof(ExpiryYear) });
        if (string.IsNullOrWhiteSpace(Cvv))
            yield return new ValidationResult("CVV is required for card payments.", new[] { nameof(Cvv) });
    }
}
