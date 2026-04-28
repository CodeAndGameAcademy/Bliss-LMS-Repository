using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.ToTable("wishlist");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.CourseId).IsRequired();

            // Unique constraint (avoid duplicate wishlist)
            builder.HasIndex(x => new { x.UserId, x.CourseId })
                   .IsUnique();

            // Unidirectional: Wishlist → User
            builder.HasOne(x => x.User)
                   .WithMany() // No navigation in User
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Unidirectional: Wishlist → Course
            builder.HasOne(x => x.Course)
                   .WithMany() // No navigation in Course
                   .HasForeignKey(x => x.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
