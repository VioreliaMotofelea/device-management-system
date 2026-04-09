using System.Net.Http.Json;
using System.Text.Json;

namespace DeviceManagement.Tests.Integration.Fixtures;

internal static class TestPayloads
{
    public static object BuildDevicePayload(
        string name,
        string manufacturer,
        string ramAmount = "8 GB",
        string processor = "Mid Chip",
        string description = "Integration test device",
        string location = "London",
        string type = "phone",
        string operatingSystem = "Android",
        string osVersion = "14") => new
    {
        Name = name,
        Manufacturer = manufacturer,
        Type = type,
        OperatingSystem = operatingSystem,
        OsVersion = osVersion,
        Processor = processor,
        RamAmount = ramAmount,
        Description = description,
        Location = location
    };

    public static object BuildCreateUserPayload(
        string email,
        string password = "Password1",
        string name = "Created User",
        string role = "Employee",
        string location = "London") => new
    {
        Email = email,
        Password = password,
        Name = name,
        Role = role,
        Location = location
    };

    public static async Task<int> CreateDeviceAsync(HttpClient client, string name, string manufacturer = "TestBrand")
    {
        var response = await client.PostAsJsonAsync(
            "/api/devices",
            BuildDevicePayload(
                name,
                manufacturer,
                description: "Assignment integration test device."));
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<JsonElement>();
        return payload.GetProperty("id").GetInt32();
    }
}
