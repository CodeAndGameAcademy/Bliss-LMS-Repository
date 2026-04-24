using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations
{
    public class CourseTargetAudienceConfiguration : IEntityTypeConfiguration<CourseTargetAudience>
    {
        public void Configure(EntityTypeBuilder<CourseTargetAudience> builder)
        {
            builder.ToTable("CourseTargetAudiences");

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}
