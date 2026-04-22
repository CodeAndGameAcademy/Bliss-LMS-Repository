using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Configurations
{
    public class CourseLanguageConfiguration : IEntityTypeConfiguration<CourseLanguage>
    {
        public void Configure(EntityTypeBuilder<CourseLanguage> builder)
        {
            builder.HasIndex(x => x.CourseLanguageName)
                   .IsUnique();

            builder.Property(x => x.CourseLanguageName)
                   .IsRequired()
                   .HasMaxLength(100);

            // Soft Delete
            builder.HasQueryFilter(x => x.DeletedAt == null);
        }
    }
}
