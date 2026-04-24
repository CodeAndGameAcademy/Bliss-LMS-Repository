using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations
{
    public class CourseModuleConfiguration : IEntityTypeConfiguration<CourseModule>
    {
        public void Configure(EntityTypeBuilder<CourseModule> builder)
        {
            builder.ToTable("CourseModules");

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.OrderIndex)
                .IsRequired();

            // Unique Order per Course
            builder.HasIndex(x => new { x.CourseId, x.OrderIndex })
                .IsUnique();

            // Soft Delete
            builder.HasQueryFilter(x => x.DeletedAt == null);

            // Relationship
            builder.HasOne(x => x.Course)
                .WithMany(x => x.CourseModules)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CourseContents)
                .WithOne(x => x.CourseModule)
                .HasForeignKey(x => x.CourseModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
