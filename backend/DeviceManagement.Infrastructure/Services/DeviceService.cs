using DeviceManagement.Application.DeviceWrite;
using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Application.Interfaces.Services;
using DeviceManagement.Application.Mapping;
using DeviceManagement.Domain.Entities;

namespace DeviceManagement.Infrastructure.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<List<DeviceResponseDto>> GetAllAsync()
    {
        var devices = await _deviceRepository.GetAllAsync();
        return devices.Select(DeviceResponseMapper.ToDto).ToList();
    }

    public async Task<DeviceResponseDto?> GetByIdAsync(int id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        return device == null ? null : DeviceResponseMapper.ToDto(device);
    }

    public async Task<DeviceResponseDto> CreateAsync(CreateDeviceDto dto)
    {
        var input = DeviceWriteInputParser.Parse(dto);

        var exists = await _deviceRepository.ExistsAsync(input.Name, input.Manufacturer);
        if (exists)
            throw new InvalidOperationException("A device with the same name and manufacturer already exists.");

        var device = MapToNewEntity(input);

        await _deviceRepository.AddAsync(device);
        await _deviceRepository.SaveChangesAsync();

        return DeviceResponseMapper.ToDto(device);
    }

    public async Task<DeviceResponseDto?> UpdateAsync(int id, UpdateDeviceDto dto)
    {
        var input = DeviceWriteInputParser.Parse(dto);

        var device = await _deviceRepository.GetByIdAsync(id);
        if (device == null)
            return null;

        var duplicate = await _deviceRepository.GetByNameAndManufacturerAsync(input.Name, input.Manufacturer);
        if (duplicate != null && duplicate.Id != id)
            throw new InvalidOperationException("Another device with the same name and manufacturer already exists.");

        ApplyInput(device, input);

        await _deviceRepository.UpdateAsync(device);
        await _deviceRepository.SaveChangesAsync();

        return DeviceResponseMapper.ToDto(device);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        if (device == null)
            return false;

        await _deviceRepository.DeleteAsync(device);
        await _deviceRepository.SaveChangesAsync();

        return true;
    }

    private static Device MapToNewEntity(DeviceWriteInput input)
    {
        return new Device
        {
            Name = input.Name,
            Manufacturer = input.Manufacturer,
            Type = input.Type,
            OperatingSystem = input.OperatingSystem,
            OsVersion = input.OsVersion,
            Processor = input.Processor,
            RamAmount = input.RamAmount,
            Description = input.Description,
            Location = input.Location
        };
    }

    private static void ApplyInput(Device device, DeviceWriteInput input)
    {
        device.Name = input.Name;
        device.Manufacturer = input.Manufacturer;
        device.Type = input.Type;
        device.OperatingSystem = input.OperatingSystem;
        device.OsVersion = input.OsVersion;
        device.Processor = input.Processor;
        device.RamAmount = input.RamAmount;
        device.Description = input.Description;
        device.Location = input.Location;
    }
}
