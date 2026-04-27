using LMS.Infrastructure.Data;
using LMS.StudentAPI.DTOs.Slider;
using LMS.StudentAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.StudentAPI.Services
{
    public class SliderService : ISliderService
    {
        private readonly ApplicationDbContext _context;

        public SliderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SliderDto>> GetAllAsync()
        {
            var sliders = await _context.Sliders
                .OrderBy(x => x.OrderIndex)
                .ToListAsync();

            return sliders.Select(x => new SliderDto
            {
                Id = x.Id,
                Image = x.Image,
                OrderIndex = x.OrderIndex
            }).ToList();
        }
    }
}
