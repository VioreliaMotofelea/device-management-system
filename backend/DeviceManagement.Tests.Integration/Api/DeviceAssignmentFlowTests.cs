using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class DeviceAssignmentFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DeviceAssignmentFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Unassign_ByDifferentUser_ReturnsForbidden()
    {
        using var client = _factory.CreateClient();

        var userA = await RegisterAndLoginAsync(client, "assign-a");
        var userB = await RegisterAndLoginAsync(client, "assign-b");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userA.Token);
        var deviceId = await CreateDeviceAsync(client, $"assign-{Guid.NewGuid():N}".Substring(0, 12));
        var assignResp = await client.PostAsync($"/api/devices/{deviceId}/assign", null);
        assignResp.EnsureSuccessStatusCode();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userB.Token);
        var unassignResp = await client.PostAsync($"/api/devices/{deviceId}/unassign", null);

        Assert.Equal(HttpStatusCode.Forbidden, unassignResp.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_WithAssignedDevice_ReturnsConflict()
    {
        using var client = _factory.CreateClient();

        var user = await RegisterAndLoginAsync(client, "delete-assigned");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

        var deviceId = await CreateDeviceAsync(client, $"del-{Guid.NewGuid():N}".Substring(0, 12));
        var assignResp = await client.PostAsync($"/api/devices/{deviceId}/assign", null);
        assignResp.EnsureSuccessStatusCode();

        var deleteResp = await client.DeleteAsync($"/api/users/{user.UserId}");

        Assert.Equal(HttpStatusCode.Conflict, deleteResp.StatusCode);
    }

    private static async Task<(string Token, int UserId)> RegisterAndLoginAsync(HttpClient client, string prefix)
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
        var token = payload.GetProperty("accessToken").GetString()
            ?? throw new InvalidOperationException("Missing access token.");
        var userId = payload.GetProperty("user").GetProperty("id").GetInt32();
        return (token, userId);
    }

    private static async Task<int> CreateDeviceAsync(HttpClient client, string name)
    {
        var resp = await client.PostAsJsonAsync("/api/devices", new
        {
            Name = name,
            Manufacturer = "TestBrand",
            Type = "phone",
            OperatingSystem = "Android",
            OsVersion = "14",
            Processor = "Mid Chip",
            RamAmount = "8 GB",
            Description = "Assignment integration test device.",
            Location = "London"
        });
        resp.EnsureSuccessStatusCode();

        var payload = await resp.Content.ReadFromJsonAsync<JsonElement>();
        return payload.GetProperty("id").GetInt32();
    }
}
