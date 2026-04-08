using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class DevicesCrudEdgeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DevicesCrudEdgeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateDevice_DuplicateNameManufacturer_ReturnsConflict()
    {
        using var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "dev-dup");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var unique = $"dup-{Guid.NewGuid():N}".Substring(0, 12);
        var body = BuildDevicePayload(unique, "BrandX");

        var first = await client.PostAsJsonAsync("/api/devices", body);
        first.EnsureSuccessStatusCode();

        var second = await client.PostAsJsonAsync("/api/devices", body);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task UpdateDevice_NotFound_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "dev-upd-miss");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PutAsJsonAsync("/api/devices/999999", BuildDevicePayload("missing", "BrandY"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDevice_NotFound_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "dev-del-miss");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync("/api/devices/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static object BuildDevicePayload(string name, string manufacturer) => new
    {
        Name = name,
        Manufacturer = manufacturer,
        Type = "phone",
        OperatingSystem = "Android",
        OsVersion = "14",
        Processor = "Mid Chip",
        RamAmount = "8 GB",
        Description = "Device CRUD edge test payload.",
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
