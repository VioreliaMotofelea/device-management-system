using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DeviceManagement.Tests.Integration.Fixtures;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class DeviceDescriptionEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DeviceDescriptionEndpointTests(CustomWebApplicationFactory factory)
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
        var token = await TestAuthHelper.RegisterAndLoginAsync(client, "desc");
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

}
