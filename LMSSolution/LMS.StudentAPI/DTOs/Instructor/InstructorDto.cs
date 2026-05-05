namespace LMS.StudentAPI.DTOs.Instructor
{
    public class InstructorDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string? Degree { get; set; }

        public string About { get; set; } = string.Empty;

        public string? CertificationSkill { get; set; }

        public string Image { get; set; } = string.Empty;
    }
}
