using DeviceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeviceManagement.Infrastructure.Data.Configurations;

public sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).HasMaxLength(255).IsRequired();
        builder.Property(d => d.Manufacturer).HasMaxLength(255).IsRequired();
        builder.Property(d => d.Type).HasMaxLength(50).IsRequired();
        builder.Property(d => d.OperatingSystem).HasMaxLength(100).IsRequired();
        builder.Property(d => d.OsVersion).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Processor).HasMaxLength(100).IsRequired();
        builder.Property(d => d.RamAmount).HasMaxLength(50).IsRequired();
        builder.Property(d => d.Description).HasColumnType("nvarchar(max)");
        builder.Property(d => d.Location).HasMaxLength(255).IsRequired();

        builder.HasOne(d => d.AssignedUser)
            .WithMany()
            .HasForeignKey(d => d.AssignedUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
