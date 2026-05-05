using LMS.StudentAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.StudentAPI.Controllers
{
    [Route("api/v1/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        // GET ALL        
        [HttpGet]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> GetHomeScreenData()
        {
            var result = await _homeService.GetHomeScreenData();
            return Ok(result);
        }
    }
}