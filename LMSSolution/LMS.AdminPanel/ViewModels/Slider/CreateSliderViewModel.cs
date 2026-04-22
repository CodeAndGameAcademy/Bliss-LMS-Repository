using System.ComponentModel.DataAnnotations;

namespace LMS.AdminPanel.ViewModels.Slider
{
    public class CreateSliderViewModel
    {
        [Required(ErrorMessage = "Image is required")]
        public IFormFile ImageFile { get; set; } = default!;

        [Required(ErrorMessage = "OrderIndex is required")]
        [Range(1, int.MaxValue, ErrorMessage = "OrderIndex must be greater than 0")]
        public int OrderIndex { get; set; }
    }
}
