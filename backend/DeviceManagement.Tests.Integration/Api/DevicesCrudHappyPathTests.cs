using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class DevicesCrudHappyPathTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DevicesCrudHappyPathTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeviceCrud_CreateGetUpdate_Works()
    {
        using var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "dev-happy");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var name = $"happy-{Guid.NewGuid():N}".Substring(0, 12);
        var createResp = await client.PostAsJsonAsync("/api/devices", BuildPayload(name, "BrandHappy", "8 GB"));
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

        var created = await createResp.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetInt32();

        var getResp = await client.GetAsync($"/api/devices/{id}");
        getResp.EnsureSuccessStatusCode();
        var fetched = await getResp.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(name, fetched.GetProperty("name").GetString());

        var updateResp = await client.PutAsJsonAsync($"/api/devices/{id}", BuildPayload(name, "BrandHappy", "12 GB"));
        updateResp.EnsureSuccessStatusCode();
        var updated = await updateResp.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("12 GB", updated.GetProperty("ramAmount").GetString());
    }

    private static object BuildPayload(string name, string manufacturer, string ramAmount) => new
    {
        Name = name,
        Manufacturer = manufacturer,
        Type = "phone",
        OperatingSystem = "Android",
        OsVersion = "14",
        Processor = "Mid Chip",
        RamAmount = ramAmount,
        Description = "Happy CRUD path device.",
        Location = "London"
    };

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
