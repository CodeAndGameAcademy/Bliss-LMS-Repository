using LMS.Domain.Entities;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            if (await context.Users.AnyAsync(u => u.Role == Role.ADMIN))
                return;

            var admin = new User
            {
                MobileNumber = "9999999999",
                Email = "admin@gmail.com",
                FullName = "System Admin",
                Role = Role.ADMIN,
                Image = "uploads/default/admin.png",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@123"),
                IsActive = true,
                PrimaryDeviceId = "Admin",
                PrimaryDeviceInfo = "Admin",
                SecondaryDeviceId = "Admin",
                SecondaryDeviceInfo = "Admin",                
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
