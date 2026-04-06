using DeviceManagement.Application.DTOs.Devices;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Domain.Constants;

namespace DeviceManagement.Application.Validators.Devices;

public static class GenerateDeviceDescriptionRequestValidator
{
    public static void Validate(GenerateDeviceDescriptionRequestDto? dto)
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

        var typeNormalized = dto.Type.Trim().ToLowerInvariant();
        if (!DeviceTypes.IsAllowed(typeNormalized))
            throw new ValidationException($"Type must be '{DeviceTypes.Phone}' or '{DeviceTypes.Tablet}'.");
    }
}
