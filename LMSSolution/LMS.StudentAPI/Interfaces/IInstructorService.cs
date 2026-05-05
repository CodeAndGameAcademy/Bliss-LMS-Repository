using LMS.StudentAPI.DTOs.Instructor;

namespace LMS.StudentAPI.Interfaces
{
    public interface IInstructorService
    {
        Task<List<InstructorDto>> GetAllAsync();
        Task<InstructorDto?> GetByIdAsync(Guid id);
    }
}
