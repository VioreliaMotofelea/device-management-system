using DeviceManagement.Application.DTOs.Auth;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Application.Interfaces.Services;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

namespace DeviceManagement.Tests.Unit.Services;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_CreatesUserAndReturnsToken()
    {
        var repo = new FakeUserRepository();
        var sut = new AuthService(repo, new FakePasswordHasher(), new FakeJwtTokenService());

        var response = await sut.RegisterAsync(new RegisterRequestDto
        {
            Email = "  New.User@Example.com ",
            Password = "Password1",
            ConfirmPassword = "Password1"
        });

        Assert.Equal("token-for-new.user@example.com", response.AccessToken);
        Assert.Equal("new.user@example.com", response.User.Email);
        Assert.Equal("new.user", response.User.Name);
    }

    [Fact]
    public async Task RegisterAsync_ThrowsConflict_WhenEmailAlreadyExists()
    {
        var repo = new FakeUserRepository { ExistingByEmail = true };
        var sut = new AuthService(repo, new FakePasswordHasher(), new FakeJwtTokenService());

        await Assert.ThrowsAsync<ConflictException>(() => sut.RegisterAsync(new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "Password1",
            ConfirmPassword = "Password1"
        }));
    }

    [Fact]
    public async Task LoginAsync_ThrowsUnauthorized_WhenUserMissing()
    {
        var repo = new FakeUserRepository { UserByEmail = null };
        var sut = new AuthService(repo, new FakePasswordHasher(), new FakeJwtTokenService());

        await Assert.ThrowsAsync<UnauthorizedException>(() => sut.LoginAsync(new LoginRequestDto
        {
            Email = "missing@example.com",
            Password = "Password1"
        }));
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_WhenPasswordValid()
    {
        var user = new User
        {
            Id = 42,
            Email = "user@example.com",
            FullName = "User Name",
            Role = "User",
            PasswordHash = "hashed:Password1",
            Location = "Unspecified"
        };
        var repo = new FakeUserRepository { UserByEmail = user };
        var sut = new AuthService(repo, new FakePasswordHasher(), new FakeJwtTokenService());

        var response = await sut.LoginAsync(new LoginRequestDto
        {
            Email = "user@example.com",
            Password = "Password1"
        });

        Assert.Equal("token-for-user@example.com", response.AccessToken);
        Assert.Equal(42, response.User.Id);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public bool ExistingByEmail { get; set; }
        public User? UserByEmail { get; set; }

        public Task<List<User>> GetAllAsync() => Task.FromResult(new List<User>());
        public Task<User?> GetByIdAsync(int id) => Task.FromResult<User?>(null);
        public Task<User?> GetByEmailAsync(string normalizedEmail) => Task.FromResult(UserByEmail);
        public Task<bool> ExistsByEmailAsync(string normalizedEmail, int? exceptUserId = null) => Task.FromResult(ExistingByEmail);
        public Task AddAsync(User user)
        {
            UserByEmail = user;
            return Task.CompletedTask;
        }
        public Task UpdateAsync(User user) => Task.CompletedTask;
        public Task DeleteAsync(User user) => Task.CompletedTask;
        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class FakePasswordHasher : IPasswordHasher<User>
    {
        public string HashPassword(User user, string password) => $"hashed:{password}";

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
            => hashedPassword == $"hashed:{providedPassword}"
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
    }

    private sealed class FakeJwtTokenService : IJwtTokenService
    {
        public (string Token, DateTime ExpiresAtUtc) CreateAccessToken(User user)
            => ($"token-for-{user.Email}", DateTime.UtcNow.AddHours(1));
    }
}
