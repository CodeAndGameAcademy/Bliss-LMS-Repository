using LMS.Infrastructure.Data;
using LMS.StudentAPI.DTOs.Category;
using LMS.StudentAPI.DTOs.Course;
using LMS.StudentAPI.DTOs.Home;
using LMS.StudentAPI.DTOs.Instructor;
using LMS.StudentAPI.DTOs.Slider;
using LMS.StudentAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.StudentAPI.Services
{
    public class HomeService : IHomeService
    {
        private readonly ApplicationDbContext _context;

        public HomeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HomeDto> GetHomeScreenData()
        {
            var sliders = await _context.Sliders
                .OrderBy(x => x.OrderIndex)
                .Select(x => new SliderDto
                {
                    Id = x.Id,
                    Image = x.Image,
                    OrderIndex = x.OrderIndex
                }).ToListAsync();

            var categories = await _context.Categories
                .Where(x => x.ParentId == null)
                .Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    DisplayName = x.DisplayName,
                    Image = x.Image,
                    ParentId = x.ParentId
                })
                .ToListAsync();

            var instructors = await _context.Instructors
                .Select(x => new InstructorDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    DisplayName = x.DisplayName,
                    Degree = x.Degree,
                    About = x.About,
                    CertificationSkill = x.CertificationSkill,
                    Image = x.Image
                })
                .ToListAsync();

            var freeCourses = await _context.Courses
                .Where(x => x.CourseType == Domain.Enums.CourseType.Free)
                .Select(x => new FreeCourseDto
                {
                    Id = x.Id,
                    DifficultyLevel = x.DifficultyLevel,
                    ShortDescription = x.ShortDescription,
                    Title = x.Title,
                    Subtitle = x.Subtitle,
                    Thumbnail = x.Thumbnail,
                    InstructorName = x.Instructor.DisplayName
                })
               .ToListAsync();

            var paidCourses = await _context.Courses
               .Where(x => x.CourseType == Domain.Enums.CourseType.Paid)
               .Select(x => new PaidCourseDto
               {
                   Id = x.Id,
                   DifficultyLevel = x.DifficultyLevel,
                   ShortDescription = x.ShortDescription,
                   Title = x.Title,
                   Subtitle = x.Subtitle,
                   Thumbnail = x.Thumbnail,
                   Price = x.Price,
                   FinalPrice = x.FinalPrice,
                   DiscountPercentage = x.DiscountPercentage,
                   InstructorName = x.Instructor.DisplayName
               })
              .ToListAsync();

            return new HomeDto
            {
                Sliders = sliders,  
                Categories = categories,   
                Instructors = instructors,
                FreeCourses = freeCourses,
                PaidCourses = paidCourses
            };
        }
    }
}
