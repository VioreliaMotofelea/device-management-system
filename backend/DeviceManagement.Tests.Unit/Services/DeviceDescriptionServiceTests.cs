using System.Net;
using System.Net.Http;
using System.Text;
using DeviceManagement.Application.DTOs.Devices;
using DeviceManagement.Application.Options;
using DeviceManagement.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace DeviceManagement.Tests.Unit.Services;

public sealed class DeviceDescriptionServiceTests
{
    [Fact]
    public async Task GenerateAsync_ReturnsTemplate_WhenApiKeyMissing()
    {
        var sut = CreateService(
            new OpenAiDescriptionOptions { ApiKey = "" },
            new HttpResponseMessage(HttpStatusCode.OK));

        var result = await sut.GenerateAsync(BuildRequest());

        Assert.Equal("template", result.Source);
        Assert.Contains("Pixel 8", result.Description);
    }

    [Fact]
    public async Task GenerateAsync_FallsBackToTemplate_WhenOpenAiFails()
    {
        var sut = CreateService(
            new OpenAiDescriptionOptions { ApiKey = "test-key" },
            new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var result = await sut.GenerateAsync(BuildRequest());

        Assert.Equal("template", result.Source);
    }

    [Fact]
    public async Task GenerateAsync_ReturnsOpenAi_WhenResponseContainsContent()
    {
        var body = """
                   {
                     "choices": [
                       { "message": { "content": "Concise generated description." } }
                     ]
                   }
                   """;
        var sut = CreateService(
            new OpenAiDescriptionOptions { ApiKey = "test-key" },
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });

        var result = await sut.GenerateAsync(BuildRequest());

        Assert.Equal("openai", result.Source);
        Assert.Equal("Concise generated description.", result.Description);
    }

    [Fact]
    public async Task GenerateAsync_FallsBackToTemplate_WhenOpenAiPayloadMalformed()
    {
        var malformed = "{ not-json";
        var sut = CreateService(
            new OpenAiDescriptionOptions { ApiKey = "test-key" },
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(malformed, Encoding.UTF8, "application/json")
            });

        var result = await sut.GenerateAsync(BuildRequest());

        Assert.Equal("template", result.Source);
        Assert.Contains("Pixel 8", result.Description);
    }

    private static DeviceDescriptionService CreateService(OpenAiDescriptionOptions options, HttpResponseMessage response)
    {
        var handler = new StubHttpMessageHandler(response);
        var client = new HttpClient(handler);
        var factory = new StubHttpClientFactory(client);
        return new DeviceDescriptionService(
            factory,
            Options.Create(options),
            NullLogger<DeviceDescriptionService>.Instance);
    }

    private static GenerateDeviceDescriptionRequestDto BuildRequest() => new()
    {
        Name = "Pixel 8",
        Manufacturer = "Google",
        Type = "phone",
        OperatingSystem = "Android",
        OsVersion = "14",
        Processor = "Tensor G3",
        RamAmount = "8 GB"
    };

    private sealed class StubHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _client;
        public StubHttpClientFactory(HttpClient client) => _client = client;
        public HttpClient CreateClient(string name) => _client;
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public StubHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_response);
    }
}
