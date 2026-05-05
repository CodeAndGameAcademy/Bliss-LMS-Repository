using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations
{
    public class SliderConfiguration : IEntityTypeConfiguration<Slider>
    {
        public void Configure(EntityTypeBuilder<Slider> builder)
        {
            // Table Name
            builder.ToTable("Sliders");

            // Primary Key
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaseUrl)
                .HasMaxLength(500);

            // Properties
            builder.Property(x => x.Image)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.OrderIndex)
                .IsRequired();

            // Unique OrderIndex (Important for sorting consistency)
            builder.HasIndex(x => x.OrderIndex)
                .IsUnique();
        }
    }
}
