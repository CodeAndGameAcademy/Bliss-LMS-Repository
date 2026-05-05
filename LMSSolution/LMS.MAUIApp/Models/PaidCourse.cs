using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.MAUIApp.Models
{
    public class PaidCourse
    {
        public string? Thumbnail { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Instructor { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
