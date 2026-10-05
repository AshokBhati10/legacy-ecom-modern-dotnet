namespace LegacyEcom.Domain.Entities;

/// <summary>
/// Shopper profile linked to an ASP.NET Core Identity user via <see cref="UserId"/>.
/// Created automatically on registration (see <c>AccountService</c>).
/// </summary>
public class Customer
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime CreatedDate { get; set; }

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
