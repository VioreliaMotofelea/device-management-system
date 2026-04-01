using Microsoft.EntityFrameworkCore;
using DeviceManagement.Domain.Entities;

namespace DeviceManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Device> Devices => Set<Device>();
}
