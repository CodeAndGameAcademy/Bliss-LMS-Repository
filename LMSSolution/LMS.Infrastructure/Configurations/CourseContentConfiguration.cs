using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations
{
    public class CourseContentConfiguration : IEntityTypeConfiguration<CourseContent>
    {
        public void Configure(EntityTypeBuilder<CourseContent> builder)
        {
            builder.ToTable("CourseContents");

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.OrderIndex)
                .IsRequired();

            builder.Property(x => x.ContentLengthInMinutes)
                .IsRequired();

            builder.Property(x => x.YoutubeVideoURL)
                .HasMaxLength(500);

            builder.Property(x => x.ContentFile)
                .HasMaxLength(500);

            builder.Property(x => x.IsFreePreview)
                .HasDefaultValue(false);

            builder.Property(x => x.CourseContentType)
                .IsRequired();

            // Soft Delete
            builder.HasQueryFilter(x => x.DeletedAt == null);

            // Unique Order per Module
            builder.HasIndex(x => new { x.CourseModuleId, x.OrderIndex })
                .IsUnique();
        }
    }
}
