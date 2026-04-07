using DeviceManagement.Application.DeviceWrite;
using DeviceManagement.Application.DTOs.Devices;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Application.Interfaces.Services;
using DeviceManagement.Application.Mapping;
using DeviceManagement.Domain.Entities;
using System.Text;

namespace DeviceManagement.Infrastructure.Services;

public class DeviceService : IDeviceService
{
    private const int NameWeight = 8;
    private const int ManufacturerWeight = 5;
    private const int ProcessorWeight = 3;
    private const int RamWeight = 2;

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

    public async Task<List<DeviceResponseDto>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ValidationException("Search query is required.");

        var tokens = Tokenize(query);
        if (tokens.Count == 0)
            throw new ValidationException("Search query must contain letters or numbers.");

        var devices = await _deviceRepository.GetAllAsync();

        var ranked = devices
            .Select(device => new
            {
                Device = device,
                Score = CalculateScore(device, tokens)
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Device.Id)
            .Select(x => DeviceResponseMapper.ToDto(x.Device))
            .ToList();

        return ranked;
    }

    public async Task<DeviceResponseDto> GetByIdAsync(int id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        if (device == null)
            throw new NotFoundException($"Device with id {id} was not found.");

        return DeviceResponseMapper.ToDto(device);
    }

    public async Task<DeviceResponseDto> CreateAsync(CreateDeviceDto dto)
    {
        var input = DeviceWriteInputParser.Parse(dto);

        var exists = await _deviceRepository.ExistsAsync(input.Name, input.Manufacturer);
        if (exists)
            throw new ConflictException("A device with the same name and manufacturer already exists.");

        var device = MapToNewEntity(input);

        await _deviceRepository.AddAsync(device);
        await _deviceRepository.SaveChangesAsync();

        return DeviceResponseMapper.ToDto(device);
    }

    public async Task<DeviceResponseDto> UpdateAsync(int id, UpdateDeviceDto dto)
    {
        var input = DeviceWriteInputParser.Parse(dto);

        var device = await _deviceRepository.GetByIdAsync(id);
        if (device == null)
            throw new NotFoundException($"Device with id {id} was not found.");

        var duplicate = await _deviceRepository.GetByNameAndManufacturerAsync(input.Name, input.Manufacturer);
        if (duplicate != null && duplicate.Id != id)
            throw new ConflictException("Another device with the same name and manufacturer already exists.");

        ApplyInput(device, input);

        await _deviceRepository.UpdateAsync(device);
        await _deviceRepository.SaveChangesAsync();

        return DeviceResponseMapper.ToDto(device);
    }

    public async Task DeleteAsync(int id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        if (device == null)
            throw new NotFoundException($"Device with id {id} was not found.");

        await _deviceRepository.DeleteAsync(device);
        await _deviceRepository.SaveChangesAsync();
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

    private static int CalculateScore(Device device, IReadOnlyCollection<string> tokens)
    {
        var name = NormalizeForSearch(device.Name);
        var manufacturer = NormalizeForSearch(device.Manufacturer);
        var processor = NormalizeForSearch(device.Processor);
        var ram = NormalizeForSearch(device.RamAmount);

        var score = 0;
        foreach (var token in tokens)
        {
            if (ContainsToken(name, token))
                score += NameWeight;
            if (ContainsToken(manufacturer, token))
                score += ManufacturerWeight;
            if (ContainsToken(processor, token))
                score += ProcessorWeight;
            if (ContainsToken(ram, token))
                score += RamWeight;
        }

        return score;
    }

    private static List<string> Tokenize(string value)
    {
        return NormalizeForSearch(value)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct()
            .ToList();
    }

    private static bool ContainsToken(string normalizedField, string token)
    {
        return normalizedField.Contains(token, StringComparison.Ordinal);
    }

    private static string NormalizeForSearch(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var sb = new StringBuilder(value.Length);
        var lastWasSpace = true;

        foreach (var ch in value)
        {
            if (char.IsLetterOrDigit(ch))
            {
                sb.Append(char.ToLowerInvariant(ch));
                lastWasSpace = false;
                continue;
            }

            if (!lastWasSpace)
            {
                sb.Append(' ');
                lastWasSpace = true;
            }
        }

        return sb.ToString().Trim();
    }
}
