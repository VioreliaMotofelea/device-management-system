using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DeviceManagement.Api.Extensions;
using DeviceManagement.Application.Exceptions;

namespace DeviceManagement.Tests.Unit.Security;

public sealed class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetRequiredUserId_ReadsNameIdentifierClaim()
    {
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "12")], "test"));

        var id = principal.GetRequiredUserId();

        Assert.Equal(12, id);
    }

    [Fact]
    public void GetRequiredUserId_ReadsJwtSubClaimWhenNameIdentifierMissing()
    {
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(JwtRegisteredClaimNames.Sub, "34")], "test"));

        var id = principal.GetRequiredUserId();

        Assert.Equal(34, id);
    }

    [Fact]
    public void GetRequiredUserId_ThrowsUnauthorized_WhenMissingOrInvalid()
    {
        var missing = new ClaimsPrincipal(new ClaimsIdentity([], "test"));
        var invalid = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "abc")], "test"));

        Assert.Throws<UnauthorizedException>(() => missing.GetRequiredUserId());
        Assert.Throws<UnauthorizedException>(() => invalid.GetRequiredUserId());
    }
}
