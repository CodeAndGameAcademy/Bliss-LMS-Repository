using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Courses");

            // Required Fields
            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Slug).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Subtitle).IsRequired().HasMaxLength(300);

            builder.Property(x => x.ShortDescription).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.LongDescription).IsRequired();

            builder.Property(x => x.Thumbnail)
                .IsRequired()
                .HasMaxLength(500)
                .HasDefaultValue("uploads/default/course_thumbnail.png");

            builder.Property(x => x.IsSequentialAccess)
                .HasDefaultValue(true);

            // Pricing
            builder.Property(x => x.Price).HasColumnType("decimal(10,2)");
            builder.Property(x => x.FinalPrice).HasColumnType("decimal(10,2)");
            builder.Property(x => x.DiscountPercentage).HasColumnType("decimal(5,2)");

            // Enums
            builder.Property(x => x.CourseType).IsRequired();
            builder.Property(x => x.DifficultyLevel).IsRequired();
            builder.Property(x => x.CourseStatus).IsRequired();

            // Unique
            builder.HasIndex(x => x.Slug).IsUnique();

            // Soft Delete
            builder.HasQueryFilter(x => x.DeletedAt == null);

            // Relationships

            builder.HasOne(c => c.CourseLanguage)
               .WithMany()
               .HasForeignKey(c => c.CourseLanguageId)
               .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(x => x.CourseModules)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CourseLearningOutcomes)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId);

            builder.HasMany(x => x.CourseRequirements)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId);

            builder.HasMany(x => x.CourseTargetAudiences)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId);
        }
    }
}
