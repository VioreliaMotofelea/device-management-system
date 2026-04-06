namespace DeviceManagement.Application.DTOs.Devices;

public sealed class GenerateDeviceDescriptionRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string OsVersion { get; set; } = string.Empty;
    public string Processor { get; set; } = string.Empty;
    public string RamAmount { get; set; } = string.Empty;
}
