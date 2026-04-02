namespace DeviceManagement.Application.DTOs;

public class UpdateUserDto
{
    public string Email { get; set; } = null!;
    public string? Password { get; set; }
    public string Name { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Location { get; set; } = null!;
}
