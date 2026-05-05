using LMS.MAUIApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.MAUIApp.ViewModels
{
    public class HomeViewModel
    {
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Instructor> Instructors { get; set; }
        public ObservableCollection<PaidCourse> PaidCourses { get; set; }
        public ObservableCollection<FreeCourse> FreeCourses { get; set; }

        public HomeViewModel()
        {
            Categories = new ObservableCollection<Category>
            {
                new Category { Icon = "bell.png", Title = "Game Development", Courses = "6+ Courses" },
                new Category { Icon = "bell.png", Title = "Design", Courses = "4+ Courses" },
                new Category { Icon = "bell.png", Title = "Marketing", Courses = "5+ Courses" },
                new Category { Icon = "bell.png", Title = "Business", Courses = "3+ Courses" }
            };

            Instructors = new ObservableCollection<Instructor>
            {
                new Instructor { Image = "bell.png", DisplayName = "Viru Paksha Das", Courses = "20 Courses" },
                new Instructor { Image = "bell.png", DisplayName = "Dhir Dhruv Das", Courses = "10 Courses" },
                new Instructor { Image = "bell.png", DisplayName = "Mahesh Gurjar", Courses = "30 Courses" },
                new Instructor { Image = "bell.png", DisplayName = "Param Gurjar", Courses = "40 Courses" }
            };

            PaidCourses = new ObservableCollection<PaidCourse>
            {
                new PaidCourse { Thumbnail = "banner2.png", Title = "Complete C Programming", Subtitle = "Complete C Programming With 200+ Practical Examples", Instructor = "Viru Paksha Das", Price = 500, FinalPrice = 400, DiscountPercentage = 20 },
                new PaidCourse { Thumbnail = "banner1.png", Title = "Complete C Programming", Subtitle = "Complete C Programming With 200+ Practical Examples", Instructor = "Viru Paksha Das", Price = 500, FinalPrice = 400, DiscountPercentage = 20 },
                new PaidCourse { Thumbnail = "banner2.png", Title = "Complete C Programming", Subtitle = "Complete C Programming With 200+ Practical Examples", Instructor = "Viru Paksha Das", Price = 500, FinalPrice = 400, DiscountPercentage = 20 },
                new PaidCourse { Thumbnail = "banner1.png", Title = "Complete C Programming", Subtitle = "Complete C Programming With 200+ Practical Examples", Instructor = "Viru Paksha Das", Price = 500, FinalPrice = 400, DiscountPercentage = 20 }
            };

            FreeCourses = new ObservableCollection<FreeCourse>
            {
                new FreeCourse { Thumbnail = "banner1.png", Title = "Complete C Programming", Subtitle = "Complete C Programming With 200+ Practical Examples", Instructor = "Viru Paksha Das" },
                new FreeCourse { Thumbnail = "banner2.png", Title = "Complete C Programming", Subtitle = "Complete C Programming With 200+ Practical Examples", Instructor = "Viru Paksha Das" },
                new FreeCourse { Thumbnail = "banner1.png", Title = "Complete C Programming", Subtitle = "Complete C Programming With 200+ Practical Examples", Instructor = "Viru Paksha Das" },
                new FreeCourse { Thumbnail = "banner2.png", Title = "Complete C Programming", Subtitle = "Complete C Programming With 200+ Practical Examples", Instructor = "Viru Paksha Das" }
            };
        }
    }
}
