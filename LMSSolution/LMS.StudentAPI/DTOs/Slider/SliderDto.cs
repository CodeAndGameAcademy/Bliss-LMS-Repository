namespace LMS.StudentAPI.DTOs.Slider
{
    public class SliderDto
    {
        public Guid Id { get; set; }
        public string Image { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }
}
