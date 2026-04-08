using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class DevicesSearchRankingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DevicesSearchRankingTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Search_PrioritizesExactNameToken_OverPartialNameToken()
    {
        using var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var marker = $"rank{Guid.NewGuid():N}".Substring(0, 12);

        await CreateDeviceAsync(client, BuildDevice($"{marker}", "BrandA"));
        await CreateDeviceAsync(client, BuildDevice($"{marker}x", "BrandB"));

        var response = await client.GetAsync($"/api/devices/search?q={marker}");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        Assert.NotNull(items);
        Assert.True(items.Count >= 2);

        var firstName = items[0].GetProperty("name").GetString();
        Assert.Equal(marker, firstName);
    }

    [Fact]
    public async Task Search_IsCaseAndPunctuationTolerant()
    {
        using var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var marker = $"norm{Guid.NewGuid():N}".Substring(0, 12);
        await CreateDeviceAsync(client, BuildDevice($"{marker}", "BrandC", processor: "SnapDragon 8 Gen 2"));

        var response = await client.GetAsync($"/api/devices/search?q={marker.ToUpperInvariant()},8");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains(marker, body, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task CreateDeviceAsync(HttpClient client, object payload)
    {
        var create = await client.PostAsJsonAsync("/api/devices", payload);
        create.EnsureSuccessStatusCode();
    }

    private static object BuildDevice(string name, string manufacturer, string processor = "Mid Chip") => new
    {
        Name = name,
        Manufacturer = manufacturer,
        Type = "phone",
        OperatingSystem = "Android",
        OsVersion = "14",
        Processor = processor,
        RamAmount = "8 GB",
        Description = "Seeded for integration ranking test.",
        Location = "London"
    };

    private static async Task<string> RegisterAndLoginAsync(HttpClient client)
    {
        var email = $"rank.{Guid.NewGuid():N}@example.com";
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
