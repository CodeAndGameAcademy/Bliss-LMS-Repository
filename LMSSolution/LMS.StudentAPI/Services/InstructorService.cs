using LMS.Infrastructure.Data;
using LMS.StudentAPI.DTOs.Instructor;
using LMS.StudentAPI.Exceptions;
using LMS.StudentAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.StudentAPI.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly ApplicationDbContext _context;

        public InstructorService(ApplicationDbContext context)
        {
            _context = context;           
        }


        // GET ALL (Active Only)       
        public async Task<List<InstructorDto>> GetAllAsync()
        {
            return await _context.Instructors
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
        }

        // GET BY ID
        public async Task<InstructorDto?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Instructors
            .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new NotFoundException("Instructor not found");

            return new InstructorDto
            {
                Id = entity.Id,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                Degree = entity.Degree,
                About = entity.About,
                CertificationSkill = entity.CertificationSkill,
                Image = entity.Image
            };
        }
    }
}
