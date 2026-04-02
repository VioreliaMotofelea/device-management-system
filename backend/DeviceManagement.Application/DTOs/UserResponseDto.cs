namespace DeviceManagement.Application.DTOs;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Location { get; set; } = null!;
}
