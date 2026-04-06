using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DeviceManagement.Application.DTOs.Devices;
using DeviceManagement.Application.Interfaces.Services;
using DeviceManagement.Application.Options;
using DeviceManagement.Application.Validators.Devices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeviceManagement.Infrastructure.Services;

public sealed class DeviceDescriptionService : IDeviceDescriptionService
{
    private const string OpenAiSource = "openai";
    private const string TemplateSource = "template";

    private static readonly JsonSerializerOptions JsonWrite = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly JsonSerializerOptions JsonRead = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<OpenAiDescriptionOptions> _options;
    private readonly ILogger<DeviceDescriptionService> _logger;

    public DeviceDescriptionService(
        IHttpClientFactory httpClientFactory,
        IOptions<OpenAiDescriptionOptions> options,
        ILogger<DeviceDescriptionService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
        _logger = logger;
    }

    public async Task<GenerateDeviceDescriptionResponseDto> GenerateAsync(
        GenerateDeviceDescriptionRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        GenerateDeviceDescriptionRequestValidator.Validate(dto);

        var opts = _options.Value;
        if (!string.IsNullOrWhiteSpace(opts.ApiKey))
        {
            try
            {
                var llmText = await TryOpenAiAsync(dto, opts, cancellationToken).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(llmText))
                {
                    return new GenerateDeviceDescriptionResponseDto
                    {
                        Description = llmText.Trim(),
                        Source = OpenAiSource
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "OpenAI device description call failed; using template fallback.");
            }
        }

        return new GenerateDeviceDescriptionResponseDto
        {
            Description = BuildTemplateDescription(dto),
            Source = TemplateSource
        };
    }

    private static string BuildTemplateDescription(GenerateDeviceDescriptionRequestDto dto)
    {
        var type = dto.Type.Trim().ToLowerInvariant();
        var typeWord = type == "tablet" ? "tablet" : "smartphone";
        return
            $"A {typeWord} from {dto.Manufacturer.Trim()}, {dto.Name.Trim()}, running {dto.OperatingSystem.Trim()} {dto.OsVersion.Trim()} " +
            $"with a {dto.Processor.Trim()} processor and {dto.RamAmount.Trim()} RAM. Suitable for everyday business use.";
    }

    private async Task<string?> TryOpenAiAsync(
        GenerateDeviceDescriptionRequestDto dto,
        OpenAiDescriptionOptions opts,
        CancellationToken cancellationToken)
    {
        var baseUrl = NormalizeBaseUrl(opts.BaseUrl);
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", opts.ApiKey.Trim());
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.Timeout = TimeSpan.FromSeconds(45);

        var userPrompt =
            $"Write one or two short, professional sentences describing this mobile device for a business inventory. " +
            $"No bullet points, no marketing hype, no technical jargon beyond what is given.\n\n" +
            $"Name: {dto.Name.Trim()}\n" +
            $"Manufacturer: {dto.Manufacturer.Trim()}\n" +
            $"Type: {dto.Type.Trim().ToLowerInvariant()}\n" +
            $"Operating system: {dto.OperatingSystem.Trim()} {dto.OsVersion.Trim()}\n" +
            $"Processor: {dto.Processor.Trim()}\n" +
            $"RAM: {dto.RamAmount.Trim()}";

        var body = new OpenAiChatRequest
        {
            Model = string.IsNullOrWhiteSpace(opts.Model) ? "gpt-4o-mini" : opts.Model.Trim(),
            Messages =
            [
                new OpenAiChatMessage { Role = "system", Content = SystemPrompt },
                new OpenAiChatMessage { Role = "user", Content = userPrompt }
            ],
            MaxTokens = Math.Clamp(opts.MaxTokens, 32, 500),
            Temperature = Math.Clamp(opts.Temperature, 0, 1)
        };

        using var response = await client
            .PostAsJsonAsync(new Uri(new Uri(baseUrl), "chat/completions"), body, JsonWrite, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogWarning("OpenAI returned {Status}: {Body}", (int)response.StatusCode, err);
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var parsed = await JsonSerializer.DeserializeAsync<OpenAiChatCompletionResponse>(stream, JsonRead, cancellationToken)
            .ConfigureAwait(false);

        var text = parsed?.Choices?.FirstOrDefault()?.Message?.Content;
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    private static string NormalizeBaseUrl(string baseUrl)
    {
        var u = string.IsNullOrWhiteSpace(baseUrl) ? "https://api.openai.com/v1/" : baseUrl.Trim();
        if (!u.EndsWith('/'))
            u += "/";
        return u;
    }

    private const string SystemPrompt =
        "You write concise, factual device summaries for an internal IT asset catalog. " +
        "Respond with plain text only, one or two sentences.";

    private sealed class OpenAiChatRequest
    {
        public string Model { get; set; } = "";
        public List<OpenAiChatMessage> Messages { get; set; } = [];
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }
        public double Temperature { get; set; }
    }

    private sealed class OpenAiChatMessage
    {
        public string Role { get; set; } = "";
        public string Content { get; set; } = "";
    }

    private sealed class OpenAiChatCompletionResponse
    {
        public List<OpenAiChoice>? Choices { get; set; }
    }

    private sealed class OpenAiChoice
    {
        public OpenAiAssistantMessage? Message { get; set; }
    }

    private sealed class OpenAiAssistantMessage
    {
        public string? Content { get; set; }
    }
}
