using System.Net.Http.Json;
using System.Text.Json;

namespace DeviceManagement.Tests.Integration.Fixtures;

internal static class TestAuthHelper
{
    public static async Task<string> RegisterAndLoginAsync(HttpClient client, string prefix = "integration")
    {
        var (token, _) = await RegisterAndLoginWithUserAsync(client, prefix);
        return token;
    }

    public static async Task<(string Token, int UserId)> RegisterAndLoginWithUserAsync(
        HttpClient client,
        string prefix = "integration")
    {
        var email = $"{prefix}.{Guid.NewGuid():N}@example.com";
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
        var token = payload.GetProperty("accessToken").GetString()
            ?? throw new InvalidOperationException("Login response did not include accessToken.");
        var userId = payload.GetProperty("user").GetProperty("id").GetInt32();
        return (token, userId);
    }
}
