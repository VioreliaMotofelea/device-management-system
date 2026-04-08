using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class DeviceDescriptionEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DeviceDescriptionEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GenerateDescription_WithoutToken_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/devices/generate-description", BuildRequest());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GenerateDescription_WithToken_ReturnsTemplateSource()
    {
        using var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "desc");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/devices/generate-description", BuildRequest());

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("template", payload.GetProperty("source").GetString());
    }

    private static object BuildRequest() => new
    {
        Name = "Pixel 8",
        Manufacturer = "Google",
        Type = "phone",
        OperatingSystem = "Android",
        OsVersion = "14",
        Processor = "Tensor G3",
        RamAmount = "8 GB"
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
