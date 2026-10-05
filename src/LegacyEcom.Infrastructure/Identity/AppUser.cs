using Microsoft.AspNetCore.Identity;

namespace LegacyEcom.Infrastructure.Identity;

/// <summary>
/// Application user. Email is used as the user name (same as the legacy
/// ASP.NET Identity 2.0 setup, where <c>UserName = Email</c>).
/// </summary>
public class AppUser : IdentityUser
{
}
