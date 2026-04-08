using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Infrastructure.Services;

namespace DeviceManagement.Tests.Unit.Services;

public sealed class DeviceAssignmentServiceTests
{
    [Fact]
    public async Task AssignToCurrentUser_ThrowsConflict_WhenAssignedToAnotherUser()
    {
        var repo = new FakeDeviceRepository(new Device
        {
            Id = 1,
            Name = "Phone",
            Manufacturer = "Brand",
            Type = "phone",
            OperatingSystem = "Android",
            OsVersion = "14",
            Processor = "Chip",
            RamAmount = "8 GB",
            Location = "London",
            AssignedUserId = 99
        });
        var sut = new DeviceAssignmentService(repo);

        await Assert.ThrowsAsync<ConflictException>(() => sut.AssignToCurrentUserAsync(1, 10));
    }

    [Fact]
    public async Task UnassignFromCurrentUser_ThrowsValidation_WhenNotAssigned()
    {
        var repo = new FakeDeviceRepository(new Device
        {
            Id = 2,
            Name = "Phone",
            Manufacturer = "Brand",
            Type = "phone",
            OperatingSystem = "Android",
            OsVersion = "14",
            Processor = "Chip",
            RamAmount = "8 GB",
            Location = "London",
            AssignedUserId = null
        });
        var sut = new DeviceAssignmentService(repo);

        await Assert.ThrowsAsync<ValidationException>(() => sut.UnassignFromCurrentUserAsync(2, 10));
    }

    [Fact]
    public async Task UnassignFromCurrentUser_ThrowsForbidden_WhenAssignedToDifferentUser()
    {
        var repo = new FakeDeviceRepository(new Device
        {
            Id = 3,
            Name = "Phone",
            Manufacturer = "Brand",
            Type = "phone",
            OperatingSystem = "Android",
            OsVersion = "14",
            Processor = "Chip",
            RamAmount = "8 GB",
            Location = "London",
            AssignedUserId = 101
        });
        var sut = new DeviceAssignmentService(repo);

        await Assert.ThrowsAsync<ForbiddenException>(() => sut.UnassignFromCurrentUserAsync(3, 10));
    }

    [Fact]
    public async Task AssignToCurrentUser_SetsAssignmentAndReturnsUpdated()
    {
        var device = new Device
        {
            Id = 4,
            Name = "Phone",
            Manufacturer = "Brand",
            Type = "phone",
            OperatingSystem = "Android",
            OsVersion = "14",
            Processor = "Chip",
            RamAmount = "8 GB",
            Location = "London",
            AssignedUserId = null
        };
        var repo = new FakeDeviceRepository(device);
        var sut = new DeviceAssignmentService(repo);

        var result = await sut.AssignToCurrentUserAsync(4, 10);

        Assert.Equal(10, result.AssignedUserId);
    }

    [Fact]
    public async Task AssignToCurrentUser_ThrowsNotFound_WhenDeviceMissing()
    {
        var repo = new FakeDeviceRepository(null);
        var sut = new DeviceAssignmentService(repo);

        await Assert.ThrowsAsync<NotFoundException>(() => sut.AssignToCurrentUserAsync(404, 10));
    }

    private sealed class FakeDeviceRepository : IDeviceRepository
    {
        private readonly Device? _device;

        public FakeDeviceRepository(Device? device)
        {
            _device = device;
        }

        public Task<List<Device>> GetAllAsync() => Task.FromResult(_device is null ? new List<Device>() : new List<Device> { _device });
        public Task<Device?> GetByIdAsync(int id) => Task.FromResult(_device is not null && id == _device.Id ? _device : null);
        public Task<Device?> GetByIdForUpdateAsync(int id) => Task.FromResult(_device is not null && id == _device.Id ? _device : null);
        public Task<Device?> GetByNameAndManufacturerAsync(string name, string manufacturer) => Task.FromResult<Device?>(null);
        public Task AddAsync(Device device) => Task.CompletedTask;
        public Task UpdateAsync(Device device) => Task.CompletedTask;
        public Task DeleteAsync(Device device) => Task.CompletedTask;
        public Task<bool> ExistsAsync(string name, string manufacturer) => Task.FromResult(false);
        public Task<bool> AnyAssignedToUserAsync(int userId) => Task.FromResult(false);
        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
