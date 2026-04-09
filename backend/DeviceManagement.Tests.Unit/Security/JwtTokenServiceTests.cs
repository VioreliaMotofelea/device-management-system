using System.IdentityModel.Tokens.Jwt;
using DeviceManagement.Application.Options;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace DeviceManagement.Tests.Unit.Security;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void CreateAccessToken_ReturnsReadableJwt_WithExpectedClaims()
    {
        var sut = new JwtTokenService(Options.Create(new JwtOptions
        {
            SecretKey = "ThisIsATestSecretKeyWithEnoughLength123!",
            Issuer = "DeviceManagement",
            Audience = "DeviceManagementClients",
            AccessTokenExpirationMinutes = 30
        }));

        var user = new User
        {
            Id = 7,
            Email = "user@example.com",
            FullName = "User Name",
            Role = "Employee",
            Location = "London",
            PasswordHash = "hash"
        };

        var (token, expires) = sut.CreateAccessToken(user);

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.True(expires > DateTime.UtcNow);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("7", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("user@example.com", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal("Employee", jwt.Claims.First(c => c.Type == System.Security.Claims.ClaimTypes.Role).Value);
    }
}
