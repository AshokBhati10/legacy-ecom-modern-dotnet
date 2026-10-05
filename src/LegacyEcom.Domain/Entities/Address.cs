namespace LegacyEcom.Domain.Entities;

public class Address
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? Label { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsDefault { get; set; }

    public Customer? Customer { get; set; }
}
