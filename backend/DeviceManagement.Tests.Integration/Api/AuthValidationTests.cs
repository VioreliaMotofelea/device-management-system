using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DeviceManagement.Tests.Integration.Fixtures;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class AuthValidationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthValidationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_WithMissingEmail_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();
        var payload = new { email = "", password = "Password1" };

        var response = await client.PostAsJsonAsync("/api/auth/login", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal("Email is required.", doc.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Register_WithShortPassword_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();
        var payload = new { email = "new.user@example.com", password = "123", confirmPassword = "123" };

        var response = await client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Contains("Password must be at least", doc.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Register_WithMismatchedConfirmPassword_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();
        var payload = new
        {
            email = $"new.user.{Guid.NewGuid():N}@example.com",
            password = "Password1",
            confirmPassword = "Password2"
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal(
            "Password and confirm password must match.",
            doc.RootElement.GetProperty("message").GetString());
    }
}
