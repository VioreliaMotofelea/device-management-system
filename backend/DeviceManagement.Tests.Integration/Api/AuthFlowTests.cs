using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DeviceManagement.Tests.Integration.Fixtures;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class AuthFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthFlowTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterThenLogin_ReturnsAccessToken()
    {
        using var client = _factory.CreateClient();
        var email = $"flow.{Guid.NewGuid():N}@example.com";
        const string password = "Password1";

        var registerResp = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = password,
            ConfirmPassword = password
        });
        registerResp.EnsureSuccessStatusCode();

        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });
        loginResp.EnsureSuccessStatusCode();

        var payload = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
        var token = payload.GetProperty("accessToken").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Register_WithSameEmailTwice_ReturnsConflict()
    {
        using var client = _factory.CreateClient();
        var email = $"dup.{Guid.NewGuid():N}@example.com";
        const string password = "Password1";

        var first = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = password,
            ConfirmPassword = password
        });
        first.EnsureSuccessStatusCode();

        var second = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = password,
            ConfirmPassword = password
        });

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }
}
