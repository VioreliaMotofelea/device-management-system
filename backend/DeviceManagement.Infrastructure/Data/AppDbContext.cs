using DeviceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasOne(d => d.AssignedUser)
                .WithMany()
                .HasForeignKey(d => d.AssignedUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Email).HasMaxLength(255);
            entity.Property(u => u.PasswordHash).HasMaxLength(500);
            entity.Property(u => u.FullName).HasMaxLength(255);
            entity.Property(u => u.Role).HasMaxLength(100);
            entity.Property(u => u.Location).HasMaxLength(255);
            entity.HasIndex(u => u.Email).IsUnique();
        });
    }
}
