namespace DeviceManagement.Application.DeviceWrite;

public sealed record DeviceWriteInput(
    string Name,
    string Manufacturer,
    string Type,
    string OperatingSystem,
    string OsVersion,
    string Processor,
    string RamAmount,
    string Description,
    string Location);
