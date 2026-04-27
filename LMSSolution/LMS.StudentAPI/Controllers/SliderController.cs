using LMS.StudentAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.StudentAPI.Controllers
{
    [Route("api/v1/slider")]
    [ApiController]
    public class SliderController : ControllerBase
    {
        private readonly ISliderService _sliderService;

        public SliderController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }

        [HttpGet]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _sliderService.GetAllAsync();
            return Ok(result);
        }
    }
}
