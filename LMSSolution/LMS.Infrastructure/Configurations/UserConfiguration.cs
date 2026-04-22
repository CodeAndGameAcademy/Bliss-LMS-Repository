using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.MobileNumber)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(191);

            builder.Property(x => x.PasswordHash)
                .IsRequired();

            builder.Property(x => x.Image)
                .HasMaxLength(500);

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.PrimaryDeviceId).HasMaxLength(100);
            builder.Property(x => x.SecondaryDeviceId).HasMaxLength(100);
            builder.Property(x => x.PrimaryDeviceInfo).HasMaxLength(250);
            builder.Property(x => x.SecondaryDeviceInfo).HasMaxLength(250);

            builder.Property(x => x.FailedLoginAttempts)
                .HasDefaultValue(0);

            builder.Property(x => x.LockoutEnd)
                .IsRequired(false);

            // Soft Delete
            builder.HasQueryFilter(x => x.DeletedAt == null);

            // Indexes
            builder.HasIndex(x => x.MobileNumber)
                   .IsUnique()
                   .HasDatabaseName("IX_Users_Mobile");

            builder.HasIndex(x => x.Email)
                   .IsUnique()
                   .HasDatabaseName("IX_Users_Email");

            builder.HasIndex(x => x.Role);
        }
    }
}
