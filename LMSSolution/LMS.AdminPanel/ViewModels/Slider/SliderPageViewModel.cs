namespace LMS.AdminPanel.ViewModels.Slider
{
    public class SliderPageViewModel
    {        
        public CreateSliderViewModel CreateSlider { get; set; } = new();
        public List<Domain.Entities.Slider> Sliders { get; set; } = new();
    }
}
