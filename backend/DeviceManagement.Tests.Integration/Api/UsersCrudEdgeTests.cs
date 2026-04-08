using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class UsersCrudEdgeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UsersCrudEdgeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_ReturnsConflict()
    {
        using var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "users-dup-admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var email = $"dup-user.{Guid.NewGuid():N}@example.com";
        var payload = BuildCreateUserPayload(email);

        var first = await client.PostAsJsonAsync("/api/users", payload);
        first.EnsureSuccessStatusCode();

        var second = await client.PostAsJsonAsync("/api/users", payload);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_NotFound_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "users-upd-miss");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PutAsJsonAsync("/api/users/999999", new
        {
            Email = "missing@example.com",
            Name = "Missing",
            Role = "Employee",
            Location = "London"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_NotFound_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "users-del-miss");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync("/api/users/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static object BuildCreateUserPayload(string email) => new
    {
        Email = email,
        Password = "Password1",
        Name = "Created User",
        Role = "Employee",
        Location = "London"
    };

    private static async Task<string> RegisterAndLoginAsync(HttpClient client, string prefix)
    {
        var email = $"{prefix}.{Guid.NewGuid():N}@example.com";
        const string password = "Password1";

        var registerResp = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = password
        });
        registerResp.EnsureSuccessStatusCode();

        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });
        loginResp.EnsureSuccessStatusCode();

        var payload = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
        return payload.GetProperty("accessToken").GetString()
            ?? throw new InvalidOperationException("Login response did not include accessToken.");
    }
}
