using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Domain.Constants;

namespace DeviceManagement.Application.DeviceWrite;

public static class DeviceWriteInputParser
{
    public static DeviceWriteInput Parse(CreateDeviceDto dto)
    {
        if (dto is null)
            throw new ValidationException("Request body is required.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ValidationException("Name is required.");
        if (string.IsNullOrWhiteSpace(dto.Manufacturer))
            throw new ValidationException("Manufacturer is required.");
        if (string.IsNullOrWhiteSpace(dto.Type))
            throw new ValidationException("Type is required.");
        if (string.IsNullOrWhiteSpace(dto.OperatingSystem))
            throw new ValidationException("Operating system is required.");
        if (string.IsNullOrWhiteSpace(dto.OsVersion))
            throw new ValidationException("OS version is required.");
        if (string.IsNullOrWhiteSpace(dto.Processor))
            throw new ValidationException("Processor is required.");
        if (string.IsNullOrWhiteSpace(dto.RamAmount))
            throw new ValidationException("RAM amount is required.");
        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new ValidationException("Description is required.");
        if (string.IsNullOrWhiteSpace(dto.Location))
            throw new ValidationException("Location is required.");

        var typeNormalized = dto.Type.Trim().ToLowerInvariant();
        if (!DeviceTypes.IsAllowed(typeNormalized))
            throw new ValidationException($"Type must be '{DeviceTypes.Phone}' or '{DeviceTypes.Tablet}'.");

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
