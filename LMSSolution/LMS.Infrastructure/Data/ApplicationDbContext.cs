using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LMS.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Slider> Sliders => Set<Slider>();
        public DbSet<Instructor> Instructors => Set<Instructor>();
        public DbSet<CourseLanguage> CourseLanguages => Set<CourseLanguage>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Wishlist> Wishlists => Set<Wishlist>();

        public DbSet<Course> Courses => Set<Course>();
        public DbSet<CourseModule> CourseModules => Set<CourseModule>();
        public DbSet<CourseContent> CourseContents => Set<CourseContent>();
        public DbSet<CourseCategory> CourseCategories => Set<CourseCategory>();
        public DbSet<CourseLearningOutcome> CourseLearningOutcomes => Set<CourseLearningOutcome>();
        public DbSet<CourseRequirement> CourseRequirements => Set<CourseRequirement>();
        public DbSet<CourseTargetAudience> CourseTargetAudiences => Set<CourseTargetAudience>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MySQL Charset & Collation (Production Ready)
            modelBuilder.HasCharSet("utf8mb4");
            modelBuilder.UseCollation("utf8mb4_unicode_ci");

            // Apply all Fluent Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
