using System.ComponentModel.DataAnnotations;

namespace LegacyEcom.Application.DTOs.Auth;

public record RegisterRequest
{
    [Required, MaxLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(6), MaxLength(100)]
    public string Password { get; init; } = string.Empty;

    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    public string? ConfirmPassword { get; init; }
}

// Note: validation attributes on positional records need the `property:`
// target, otherwise they land on the constructor parameter and
// Validator.TryValidateObject never sees them.
public record LoginRequest(
    [property: Required, EmailAddress, MaxLength(256)] string Email,
    [property: Required, DataType(DataType.Password), MinLength(6), MaxLength(100)] string Password,
    bool RememberMe = false);

public record UserDto(string Id, string Email, string? FirstName, string? LastName);
