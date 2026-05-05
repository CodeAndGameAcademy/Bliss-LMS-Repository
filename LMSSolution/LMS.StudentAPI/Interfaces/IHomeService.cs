using LMS.StudentAPI.DTOs.Home;

namespace LMS.StudentAPI.Interfaces
{
    public interface IHomeService
    {
        Task<HomeDto> GetHomeScreenData();
    }
}
