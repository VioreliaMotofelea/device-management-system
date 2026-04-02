using DeviceManagement.Application.DTOs;
using DeviceManagement.Domain.Entities;

namespace DeviceManagement.Application.Mapping;

public static class UserResponseMapper
{
    public static UserResponseDto ToDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.FullName,
            Role = user.Role,
            Location = user.Location
        };
    }
}
