using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Table
            builder.ToTable("Categories");

            // Properties
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(160);

            builder.Property(x => x.OrderIndex)
                .IsRequired();

            builder.Property(x => x.DisplayName)
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.BaseUrl)
                .HasMaxLength(500);

            builder.Property(x => x.Image)
                .IsRequired()
                .HasMaxLength(500);

            // Relationships (Self Reference)
            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft Delete
            builder.HasQueryFilter(x => x.DeletedAt == null);

            // =========================
            // UNIQUE CONSTRAINTS
            // =========================

            // Name unique per parent
            builder.HasIndex(x => new { x.ParentId, x.Name })
                .IsUnique();

            // OrderIndex unique per parent (covers root + child)
            builder.HasIndex(x => new { x.ParentId, x.OrderIndex })
                .IsUnique();

            // Slug global unique
            builder.HasIndex(x => x.Slug)
                .IsUnique();

            // Root-level name safety
            builder.HasIndex(x => x.Name)
                .HasFilter("ParentId IS NULL")
                .IsUnique();
        }
    }
}
