using DeviceManagement.Application.DTOs.Users;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

namespace DeviceManagement.Tests.Unit.Services;

public sealed class UserServiceTests
{
    [Fact]
    public async Task CreateAsync_ThrowsConflict_WhenEmailAlreadyExists()
    {
        var sut = CreateSut(userExists: true);

        await Assert.ThrowsAsync<ConflictException>(() => sut.CreateAsync(new CreateUserDto
        {
            Email = "user@example.com",
            Password = "Password1",
            Name = "User",
            Role = "Employee",
            Location = "London"
        }));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFound_WhenUserMissing()
    {
        var sut = CreateSut(userById: null);

        await Assert.ThrowsAsync<NotFoundException>(() => sut.UpdateAsync(7, new UpdateUserDto
        {
            Email = "user@example.com",
            Name = "User",
            Role = "Employee",
            Location = "London"
        }));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsConflict_WhenEmailTakenByAnother()
    {
        var user = new User
        {
            Id = 1,
            Email = "old@example.com",
            FullName = "Old",
            Role = "Employee",
            Location = "London",
            PasswordHash = "hash"
        };
        var sut = CreateSut(userById: user, userExists: true);

        await Assert.ThrowsAsync<ConflictException>(() => sut.UpdateAsync(1, new UpdateUserDto
        {
            Email = "taken@example.com",
            Name = "Updated",
            Role = "Manager",
            Location = "Berlin"
        }));
    }

    [Fact]
    public async Task DeleteAsync_ThrowsConflict_WhenUserHasAssignedDevices()
    {
        var user = new User
        {
            Id = 3,
            Email = "u@example.com",
            FullName = "User",
            Role = "Employee",
            Location = "London",
            PasswordHash = "hash"
        };
        var sut = CreateSut(userById: user, userHasAssignedDevices: true);

        await Assert.ThrowsAsync<ConflictException>(() => sut.DeleteAsync(3));
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedUserDto()
    {
        var sut = CreateSut();

        var result = await sut.CreateAsync(new CreateUserDto
        {
            Email = "  New.User@Example.com ",
            Password = "Password1",
            Name = " New User ",
            Role = " Employee ",
            Location = " London "
        });

        Assert.Equal("new.user@example.com", result.Email);
        Assert.Equal("New User", result.Name);
    }

    private static UserService CreateSut(
        User? userById = null,
        bool userExists = false,
        bool userHasAssignedDevices = false)
    {
        var users = new FakeUserRepository
        {
            UserById = userById,
            ExistsByEmail = userExists
        };
        var devices = new FakeDeviceRepository
        {
            AnyAssignedToUser = userHasAssignedDevices
        };
        return new UserService(users, devices, new FakePasswordHasher());
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public User? UserById { get; set; }
        public bool ExistsByEmail { get; set; }

        public Task<List<User>> GetAllAsync() => Task.FromResult(new List<User>());
        public Task<User?> GetByIdAsync(int id) => Task.FromResult(UserById);
        public Task<User?> GetByEmailAsync(string normalizedEmail) => Task.FromResult<User?>(null);
        public Task<bool> ExistsByEmailAsync(string normalizedEmail, int? exceptUserId = null) => Task.FromResult(ExistsByEmail);
        public Task AddAsync(User user)
        {
            UserById = user;
            return Task.CompletedTask;
        }
        public Task UpdateAsync(User user)
        {
            UserById = user;
            return Task.CompletedTask;
        }
        public Task DeleteAsync(User user)
        {
            UserById = null;
            return Task.CompletedTask;
        }
        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class FakeDeviceRepository : IDeviceRepository
    {
        public bool AnyAssignedToUser { get; set; }
        public Task<List<Device>> GetAllAsync() => Task.FromResult(new List<Device>());
        public Task<Device?> GetByIdAsync(int id) => Task.FromResult<Device?>(null);
        public Task<Device?> GetByIdForUpdateAsync(int id) => Task.FromResult<Device?>(null);
        public Task<Device?> GetByNameAndManufacturerAsync(string name, string manufacturer) => Task.FromResult<Device?>(null);
        public Task AddAsync(Device device) => Task.CompletedTask;
        public Task UpdateAsync(Device device) => Task.CompletedTask;
        public Task DeleteAsync(Device device) => Task.CompletedTask;
        public Task<bool> ExistsAsync(string name, string manufacturer) => Task.FromResult(false);
        public Task<bool> AnyAssignedToUserAsync(int userId) => Task.FromResult(AnyAssignedToUser);
        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class FakePasswordHasher : IPasswordHasher<User>
    {
        public string HashPassword(User user, string password) => $"hashed:{password}";
        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
            => PasswordVerificationResult.Success;
    }
}
