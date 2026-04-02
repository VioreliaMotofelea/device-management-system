namespace DeviceManagement.Application.DTOs;

public class CreateUserDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Location { get; set; } = null!;
}
