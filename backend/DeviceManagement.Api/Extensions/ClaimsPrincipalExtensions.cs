using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DeviceManagement.Application.Exceptions;

namespace DeviceManagement.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetRequiredUserId(this ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (sub is null || !int.TryParse(sub, out var userId))
            throw new UnauthorizedException("Authenticated user id is missing or invalid.");

        return userId;
    }
}
