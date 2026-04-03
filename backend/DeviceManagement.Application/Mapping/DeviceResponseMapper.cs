using DeviceManagement.Application.DTOs.Devices;
using DeviceManagement.Domain.Entities;

namespace DeviceManagement.Application.Mapping;

public static class DeviceResponseMapper
{
    public static DeviceResponseDto ToDto(Device device)
    {
        return new DeviceResponseDto
        {
            Id = device.Id,
            Name = device.Name,
            Manufacturer = device.Manufacturer,
            Type = device.Type,
            OperatingSystem = device.OperatingSystem,
            OsVersion = device.OsVersion,
            Processor = device.Processor,
            RamAmount = device.RamAmount,
            Description = device.Description,
            Location = device.Location
        };
    }
}
