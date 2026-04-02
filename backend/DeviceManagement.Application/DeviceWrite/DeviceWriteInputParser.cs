using DeviceManagement.Application.DTOs;
using DeviceManagement.Domain.Constants;

namespace DeviceManagement.Application.DeviceWrite;

public static class DeviceWriteInputParser
{
    public static DeviceWriteInput Parse(CreateDeviceDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(dto.Manufacturer))
            throw new ArgumentException("Manufacturer is required.");
        if (string.IsNullOrWhiteSpace(dto.Type))
            throw new ArgumentException("Type is required.");
        if (string.IsNullOrWhiteSpace(dto.OperatingSystem))
            throw new ArgumentException("Operating system is required.");
        if (string.IsNullOrWhiteSpace(dto.OsVersion))
            throw new ArgumentException("OS version is required.");
        if (string.IsNullOrWhiteSpace(dto.Processor))
            throw new ArgumentException("Processor is required.");
        if (string.IsNullOrWhiteSpace(dto.RamAmount))
            throw new ArgumentException("RAM amount is required.");
        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new ArgumentException("Description is required.");
        if (string.IsNullOrWhiteSpace(dto.Location))
            throw new ArgumentException("Location is required.");

        var typeNormalized = dto.Type.Trim().ToLowerInvariant();
        if (!DeviceTypes.IsAllowed(typeNormalized))
            throw new ArgumentException($"Type must be '{DeviceTypes.Phone}' or '{DeviceTypes.Tablet}'.");

        return new DeviceWriteInput(
            dto.Name.Trim(),
            dto.Manufacturer.Trim(),
            typeNormalized,
            dto.OperatingSystem.Trim(),
            dto.OsVersion.Trim(),
            dto.Processor.Trim(),
            dto.RamAmount.Trim(),
            dto.Description.Trim(),
            dto.Location.Trim());
    }
}
