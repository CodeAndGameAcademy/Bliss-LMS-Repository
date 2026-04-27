using LMS.StudentAPI.DTOs.Slider;

namespace LMS.StudentAPI.Interfaces
{
    public interface ISliderService
    {
        Task<List<SliderDto>> GetAllAsync();
    }
}
