using System.ComponentModel.DataAnnotations;

namespace LMS.AdminPanel.ViewModels.Slider
{
    public class UpdateSliderViewModel
    {        
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImage { get; set; }

        [Required(ErrorMessage = "OrderIndex is required")]
        [Range(1, int.MaxValue, ErrorMessage = "OrderIndex must be greater than 0")]
        public int OrderIndex { get; set; }
    }
}
