namespace DeviceManagement.Application.DTOs.Devices;

public sealed class GenerateDeviceDescriptionResponseDto
{
    public string Description { get; set; } = string.Empty;

    // template (local rules) or openai (LLM)
    public string Source { get; set; } = string.Empty;
}
