namespace DeviceManagement.Application.DTOs;

public class CreateDeviceDto
{
    public string Name { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string OperatingSystem { get; set; } = null!;
    public string OsVersion { get; set; } = null!;
    public string Processor { get; set; } = null!;
    public string RamAmount { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
}
