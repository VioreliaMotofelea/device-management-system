using DeviceManagement.Application.DTOs.Users;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Application.Interfaces.Services;
using DeviceManagement.Application.Mapping;
using DeviceManagement.Application.UserWrite;
using DeviceManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DeviceManagement.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IDeviceRepository deviceRepository,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _deviceRepository = deviceRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserResponseDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(UserResponseMapper.ToDto).ToList();
    }

    public async Task<UserResponseDto> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new NotFoundException($"User with id {id} was not found.");

        return UserResponseMapper.ToDto(user);
    }

    public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)
    {
        var input = UserInputParser.ParseCreate(dto);

        if (await _userRepository.ExistsByEmailAsync(input.Email))
            throw new ConflictException("A user with this email already exists.");

        var user = new User
        {
            Email = input.Email,
            FullName = input.Name,
            Role = input.Role,
            Location = input.Location
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, input.Password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return UserResponseMapper.ToDto(user);
    }

    public async Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto dto)
    {
        var input = UserInputParser.ParseUpdate(dto);

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new NotFoundException($"User with id {id} was not found.");

        if (await _userRepository.ExistsByEmailAsync(input.Email, id))
            throw new ConflictException("Another user with this email already exists.");

        user.Email = input.Email;
        user.FullName = input.Name;
        user.Role = input.Role;
        user.Location = input.Location;

        if (input.Password is not null)
            user.PasswordHash = _passwordHasher.HashPassword(user, input.Password);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return UserResponseMapper.ToDto(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new NotFoundException($"User with id {id} was not found.");

        if (await _deviceRepository.AnyAssignedToUserAsync(id))
            throw new ConflictException("Cannot delete a user who still has devices assigned.");

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}
