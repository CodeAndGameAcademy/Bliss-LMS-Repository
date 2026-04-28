using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            // Table Name
            builder.ToTable("Instructors");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Degree)
                .HasMaxLength(200);

            builder.Property(x => x.About)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.CertificationSkill)
                .HasMaxLength(500);

            builder.Property(x => x.Image)
                .IsRequired()
                .HasMaxLength(300);

            // Soft Delete
            builder.HasQueryFilter(x => x.DeletedAt == null);

            builder.HasMany(x => x.Courses)
                .WithOne(x => x.Instructor)
                .HasForeignKey(x => x.InstructorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index (Optional but recommended)
            builder.HasIndex(x => x.Name);
            builder.HasIndex(x => x.DisplayName).IsUnique();
        }
    }
}
