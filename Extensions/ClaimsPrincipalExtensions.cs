using System.Security.Claims;
using Fcg.Domain.Users;

namespace Fcg.CatalogAPI.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue(ClaimTypes.Name)
            ?? user.FindFirstValue("sub");

        if (value is null || !Guid.TryParse(value, out var id))
            throw new UnauthorizedAccessException("Token sem identificação de usuário.");

        return id;
    }

    public static UserRole GetUserRole(this ClaimsPrincipal user)
    {
        var role = user.FindFirstValue(ClaimTypes.Role) ?? user.FindFirstValue("role");
        return Enum.TryParse<UserRole>(role, true, out var parsed) ? parsed : UserRole.User;
    }
}
