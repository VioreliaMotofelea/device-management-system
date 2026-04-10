using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DeviceManagement.Tests.Integration.Fixtures;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class UsersCrudEdgeTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UsersCrudEdgeTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_ReturnsConflict()
    {
        using var client = _factory.CreateClient();
        var token = await TestAuthHelper.RegisterAndLoginAsync(client, "users-dup-admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var email = $"dup-user.{Guid.NewGuid():N}@example.com";
        var payload = TestPayloads.BuildCreateUserPayload(email);

        var first = await client.PostAsJsonAsync("/api/users", payload);
        first.EnsureSuccessStatusCode();

        var second = await client.PostAsJsonAsync("/api/users", payload);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_NotFound_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        var token = await TestAuthHelper.RegisterAndLoginAsync(client, "users-upd-miss");
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
        var token = await TestAuthHelper.RegisterAndLoginAsync(client, "users-del-miss");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync("/api/users/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

}
