using DeviceManagement.Application.DTOs;

namespace DeviceManagement.Application.Interfaces.Services;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto> GetByIdAsync(int id);
    Task<UserResponseDto> CreateAsync(CreateUserDto dto);
    Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto dto);
    Task DeleteAsync(int id);
}
