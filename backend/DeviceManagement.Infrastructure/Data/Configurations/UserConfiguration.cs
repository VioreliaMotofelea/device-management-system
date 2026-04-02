using DeviceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeviceManagement.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).HasMaxLength(255).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(u => u.FullName).HasMaxLength(255).IsRequired();
        builder.Property(u => u.Role).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Location).HasMaxLength(255).IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
