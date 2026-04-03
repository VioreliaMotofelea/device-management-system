using DeviceManagement.Application.DTOs.Auth;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Application.Interfaces.Services;
using DeviceManagement.Application.Validators.Auth;
using DeviceManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DeviceManagement.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private const string DefaultRegisteredRole = "User";
    private const string DefaultRegisteredLocation = "Unspecified";

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var (email, password) = RegisterRequestValidator.Validate(dto);

        if (await _userRepository.ExistsByEmailAsync(email))
            throw new ConflictException("A user with this email already exists.");

        var displayName = DeriveDisplayNameFromEmail(email);

        var user = new User
        {
            Email = email,
            FullName = displayName,
            Role = DefaultRegisteredRole,
            Location = DefaultRegisteredLocation
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var (email, password) = LoginRequestValidator.Validate(dto);

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new UnauthorizedException("Invalid email or password.");

        var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verify == PasswordVerificationResult.Failed)
            throw new UnauthorizedException("Invalid email or password.");

        return BuildAuthResponse(user);
    }

    private AuthResponseDto BuildAuthResponse(User user)
    {
        var (token, expiresAtUtc) = _jwtTokenService.CreateAccessToken(user);
        return new AuthResponseDto
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            User = new AuthenticatedUserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.FullName,
                Role = user.Role
            }
        };
    }

    private static string DeriveDisplayNameFromEmail(string normalizedEmail)
    {
        var at = normalizedEmail.IndexOf('@');
        var local = at > 0 ? normalizedEmail[..at] : normalizedEmail;
        return string.IsNullOrWhiteSpace(local) ? normalizedEmail : local;
    }
}
