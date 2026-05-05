using LMS.Infrastructure.Data;
using LMS.StudentAPI.DTOs.Course;
using LMS.StudentAPI.DTOs.CourseContent;
using LMS.StudentAPI.DTOs.CourseModule;
using LMS.StudentAPI.Exceptions;
using LMS.StudentAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.StudentAPI.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FreeCourseDto>> GetAllFreeCoursesAsync()
        {
            return await _context.Courses
                .Where(x => x.CourseType == Domain.Enums.CourseType.Free)
                .Select(x => new FreeCourseDto
                {
                    Id = x.Id,
                    DifficultyLevel = x.DifficultyLevel,
                    ShortDescription = x.ShortDescription,
                    Title = x.Title,
                    Subtitle = x.Subtitle,
                    Thumbnail = x.BaseUrl + "/" + x.Thumbnail,
                    InstructorName = x.Instructor.DisplayName
                })
               .ToListAsync();
        }

        public async Task<List<PaidCourseDto>> GetAllPaidCoursesAsync()
        {
            return await _context.Courses
               .Where(x => x.CourseType == Domain.Enums.CourseType.Paid)
               .Select(x => new PaidCourseDto
               {
                   Id = x.Id,
                   DifficultyLevel = x.DifficultyLevel,
                   ShortDescription = x.ShortDescription,
                   Title = x.Title,
                   Subtitle = x.Subtitle,
                   Thumbnail = x.BaseUrl + "/" + x.Thumbnail,
                   Price = x.Price,
                   FinalPrice = x.FinalPrice,
                   DiscountPercentage = x.DiscountPercentage,
                   InstructorName = x.Instructor.DisplayName
               })
              .ToListAsync();
        }

        public async Task<CourseDetailDto> GetByIdAsync(Guid id)
        {
            var course = await _context.Courses
                .Include(x => x.CourseModules).ThenInclude(m => m.CourseContents)
                .Include(x => x.CourseRequirements)
                .Include(x => x.CourseTargetAudiences)
                .Include(x => x.CourseLearningOutcomes)
                .Include(x => x.CourseLanguage)
                .Include(x => x.Instructor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (course == null)
                throw new NotFoundException("Course not found");

            var result = new CourseDetailDto
            {
                Id = course.Id,
                Title = course.Title,
                Subtitle = course.Subtitle,
                ShortDescription = course.ShortDescription,
                LongDescription = course.LongDescription,
                Thumbnail = course.BaseUrl + "/" + course.Thumbnail,
                DifficultyLevel = course.DifficultyLevel,
                CourseLanguage = course.CourseLanguage.CourseLanguageName,
                CourseType = course.CourseType,
                Price = course.Price,
                FinalPrice = course.FinalPrice,
                DiscountPercentage = course.DiscountPercentage,
                Instructor = course.Instructor,

                CourseLanguageOutcomes = course.CourseLearningOutcomes.Select(x => x.Value).ToList(),
                CourseRequirements = course.CourseRequirements.Select(x => x.Value).ToList(),
                CourseTargetAudiences = course.CourseTargetAudiences.Select(x => x.Value).ToList(),

                Modules = course.CourseModules.OrderBy(m => m.OrderIndex)
                .Select(m => new CourseModuleDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    OrderIndex = m.OrderIndex,

                    Contents = m.CourseContents
                        .OrderBy(c => c.OrderIndex)
                        .Select(c => new CourseContentDto
                        {
                            Id = c.Id,
                            Title = c.Title,
                            OrderIndex = c.OrderIndex,
                            CourseContentType = c.CourseContentType,
                            YoutubeVideoURL = c.YoutubeVideoURL,
                            ContentFile = c.ContentFile,
                            ContentLengthInMinutes = c.ContentLengthInMinutes,
                            IsFreePreview = c.IsFreePreview
                        }).ToList()

                }).ToList()
            };

            return result;
        }
    }
}
