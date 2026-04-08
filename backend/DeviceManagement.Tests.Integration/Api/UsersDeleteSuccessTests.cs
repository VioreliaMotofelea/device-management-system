using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class UsersDeleteSuccessTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UsersDeleteSuccessTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeleteUser_WithoutAssignedDevices_ReturnsNoContent()
    {
        using var client = _factory.CreateClient();
        var adminToken = await RegisterAndLoginAsync(client, "admin-del-ok");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var createResp = await client.PostAsJsonAsync("/api/users", new
        {
            Email = $"to.delete.{Guid.NewGuid():N}@example.com",
            Password = "Password1",
            Name = "Delete Me",
            Role = "Employee",
            Location = "London"
        });
        createResp.EnsureSuccessStatusCode();

        var payload = await createResp.Content.ReadFromJsonAsync<JsonElement>();
        var id = payload.GetProperty("id").GetInt32();

        var deleteResp = await client.DeleteAsync($"/api/users/{id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);
    }

    private static async Task<string> RegisterAndLoginAsync(HttpClient client, string prefix)
    {
        var email = $"{prefix}.{Guid.NewGuid():N}@example.com";
        const string password = "Password1";

        var registerResp = await client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });
        registerResp.EnsureSuccessStatusCode();
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        loginResp.EnsureSuccessStatusCode();

        var payload = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
        return payload.GetProperty("accessToken").GetString()
            ?? throw new InvalidOperationException("Login response did not include accessToken.");
    }
}
