using LegacyEcom.Application.DTOs.Auth;
using LegacyEcom.Application.DTOs.Orders;
using LegacyEcom.Application.Interfaces;
using LegacyEcom.Domain.Entities;
using LegacyEcom.Domain.Interfaces;

namespace LegacyEcom.Application.Services;

public class AccountService : IAccountService
{
    private readonly IOrderRepository _orders;
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(
        IOrderRepository orders,
        ICustomerRepository customers,
        IUnitOfWork unitOfWork)
    {
        _orders = orders;
        _customers = customers;
        _unitOfWork = unitOfWork;
    }

    public IReadOnlyList<OrderSummaryDto> GetOrderHistory(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return new List<OrderSummaryDto>();

        return _orders.GetByUserId(userId)
            .Select(o => new OrderSummaryDto(
                o.Id,
                o.OrderNumber,
                o.OrderDate,
                o.Status,
                o.OrderLines.Sum(l => l.Quantity),
                o.Total))
            .ToList();
    }

    public Customer EnsureCustomerForUser(string userId, string email, string firstName, string lastName)
    {
        var existing = _customers.GetByUserId(userId);
        if (existing is not null) return existing;

        var customer = new Customer
        {
            UserId = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedDate = DateTime.UtcNow
        };

        _customers.Add(customer);
        _unitOfWork.SaveChanges();

        return _customers.GetByUserId(userId)!;
    }

    public Customer? GetCustomerByUserId(string userId) =>
        _customers.GetByUserId(userId);

    public UserDto? ToUserDto(string userId, string email)
    {
        var customer = _customers.GetByUserId(userId);
        return new UserDto(userId, email, customer?.FirstName, customer?.LastName);
    }
}
