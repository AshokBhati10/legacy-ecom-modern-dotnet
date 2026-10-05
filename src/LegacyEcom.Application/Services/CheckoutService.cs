using LegacyEcom.Application.DTOs.Cart;
using LegacyEcom.Application.DTOs.Checkout;
using LegacyEcom.Application.DTOs.Orders;
using LegacyEcom.Application.Interfaces;
using LegacyEcom.Application.Pricing;
using LegacyEcom.Domain.Entities;
using LegacyEcom.Domain.Interfaces;

namespace LegacyEcom.Application.Services;

public class CheckoutService : ICheckoutService
{
    private readonly ICartService _cartService;
    private readonly IOrderRepository _orders;
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutService(
        ICartService cartService,
        IOrderRepository orders,
        ICustomerRepository customers,
        IUnitOfWork unitOfWork)
    {
        _cartService = cartService;
        _orders = orders;
        _customers = customers;
        _unitOfWork = unitOfWork;
    }

    public CheckoutSummaryDto BuildSummary(IEnumerable<SessionCartItem> items, string? shippingMethod)
    {
        var cart = _cartService.GetCart(items);
        var method = PriceCalculator.NormalizeShippingMethod(shippingMethod);
        var shipping = PriceCalculator.CalculateShippingCost(cart.SubTotal, method);
        var tax = PriceCalculator.CalculateTax(cart.SubTotal);

        return new CheckoutSummaryDto(
            cart.Items,
            cart.ItemCount,
            cart.SubTotal,
            method,
            shipping,
            tax,
            PriceCalculator.CalculateTotal(cart.SubTotal, shipping, tax));
    }

    public IReadOnlyList<ShippingOptionDto> GetShippingOptions(IEnumerable<SessionCartItem> items)
    {
        var cart = _cartService.GetCart(items);
        return PriceCalculator.GetShippingOptions(cart.SubTotal);
    }

    public ServiceResult<Order> PlaceOrder(
        IEnumerable<SessionCartItem> items,
        AddressRequest address,
        string? shippingMethod,
        string? paymentMethod,
        string? userId)
    {
        var cart = _cartService.GetCart(items);
        if (cart.Items.Count == 0)
            return ServiceResult<Order>.Fail("Your cart is empty.");

        if (address is null)
            return ServiceResult<Order>.Fail("A shipping address is required.");

        var method = PriceCalculator.NormalizeShippingMethod(shippingMethod);
        var shipping = PriceCalculator.CalculateShippingCost(cart.SubTotal, method);
        var tax = PriceCalculator.CalculateTax(cart.SubTotal);
        var total = PriceCalculator.CalculateTotal(cart.SubTotal, shipping, tax);

        Customer? customer = null;
        if (!string.IsNullOrWhiteSpace(userId))
            customer = _customers.GetByUserId(userId);

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customer?.Id,
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Status = "Placed",
            SubTotal = cart.SubTotal,
            ShippingCost = shipping,
            TaxAmount = tax,
            Total = total,
            ShippingMethod = method,
            PaymentMethod = string.IsNullOrWhiteSpace(paymentMethod) ? "CashOnDelivery" : paymentMethod,
            ShipFirstName = address.FirstName,
            ShipLastName = address.LastName,
            ShipEmail = address.Email,
            ShipPhone = address.Phone,
            ShipStreet = address.Street,
            ShipCity = address.City,
            ShipState = address.State,
            ShipPostalCode = address.PostalCode,
            ShipCountry = address.Country,
            OrderLines = cart.Items.Select(l => new OrderLine
            {
                ProductId = l.ProductId,
                VariantId = l.VariantId,
                ProductName = l.ProductName,
                VariantName = l.VariantName,
                Sku = l.Sku,
                UnitPrice = l.UnitPrice,
                Quantity = l.Quantity,
                LineTotal = l.LineTotal
            }).ToList()
        };

        _orders.Add(order);
        _unitOfWork.SaveChanges();

        var saved = _orders.GetByOrderNumber(order.OrderNumber);
        if (saved is null)
            return ServiceResult<Order>.Fail("The order could not be saved.");

        return ServiceResult<Order>.Ok(saved, "Order placed successfully.");
    }

    public OrderDto? GetOrder(int orderId)
    {
        var o = _orders.GetById(orderId);
        if (o is null) return null;

        return new OrderDto(
            o.Id, o.OrderNumber, o.OrderDate, o.Status,
            o.SubTotal, o.ShippingCost, o.TaxAmount, o.Total,
            o.ShippingMethod, o.PaymentMethod,
            o.ShipFirstName, o.ShipLastName, o.ShipEmail, o.ShipPhone,
            o.ShipStreet, o.ShipCity, o.ShipState, o.ShipPostalCode, o.ShipCountry,
            o.OrderLines.Select(l => new OrderLineDto(
                l.Id, l.ProductId, l.VariantId, l.ProductName, l.VariantName,
                l.Sku, l.UnitPrice, l.Quantity, l.LineTotal)).ToList());
    }

    public bool CanViewOrder(int orderId, string? userId, int? sessionLastOrderId)
    {
        if (sessionLastOrderId == orderId) return true;
        if (string.IsNullOrEmpty(userId)) return false;

        var order = _orders.GetById(orderId);
        return order is not null &&
               string.Equals(order.UserId, userId, StringComparison.Ordinal);
    }

    private string GenerateOrderNumber()
    {
        // LE-YYYYMMDD-XXXXXX; retried on the (unlikely) collision.
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var number = "LE-" + DateTime.UtcNow.ToString("yyyyMMdd")
                + "-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            if (_orders.GetByOrderNumber(number) is null)
                return number;
        }
        return "LE-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss")
            + "-" + Guid.NewGuid().ToString("N")[..4].ToUpperInvariant();
    }
}
