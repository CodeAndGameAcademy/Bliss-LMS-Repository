using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    public class Slider : BaseEntity
    {
        public string Image { get; set; } = string.Empty;

        public int OrderIndex { get; set; }
    }
}
