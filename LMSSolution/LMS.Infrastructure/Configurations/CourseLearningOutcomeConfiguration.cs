using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations
{
    public class CourseLearningOutcomeConfiguration : IEntityTypeConfiguration<CourseLearningOutcome>
    {
        public void Configure(EntityTypeBuilder<CourseLearningOutcome> builder)
        {
            builder.ToTable("CourseLearningOutcomes");

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}
