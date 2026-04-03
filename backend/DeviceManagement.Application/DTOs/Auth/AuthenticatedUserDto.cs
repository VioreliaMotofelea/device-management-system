namespace DeviceManagement.Application.DTOs.Auth;

public class AuthenticatedUserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Role { get; set; } = null!;
}
