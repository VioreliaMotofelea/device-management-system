namespace DeviceManagement.Application.Options;

// OpenAI settings. When ApiKey is empty, only the local template description is used.
public sealed class OpenAiDescriptionOptions
{
    public const string SectionName = "OpenAi";

    // API key; empty for template-only mode (CI, no cost)
    public string ApiKey { get; set; } = string.Empty;

    // Chat model id (gpt-4o-mini)
    public string Model { get; set; } = "gpt-4o-mini";

    // API base URL, must end before /chat/completions (https://api.openai.com/v1/)
    public string BaseUrl { get; set; } = "https://api.openai.com/v1/";

    // Max completion tokens for the short business blurb
    public int MaxTokens { get; set; } = 200;

    public double Temperature { get; set; } = 0.45;
}
