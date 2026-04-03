namespace DeviceManagement.Domain.Entities;

public class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string OperatingSystem { get; set; } = null!;
    public string OsVersion { get; set; } = null!;
    public string Processor { get; set; } = null!;
    public string RamAmount { get; set; } = null!;
    public string? Description { get; set; }
    public string Location { get; set; } = null!;
    public int? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }
}