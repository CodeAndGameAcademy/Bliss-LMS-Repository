namespace LMS.StudentAPI.DTOs.WishList
{
    public class MyWishlistDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal DiscountPercentage { get; set; }

        public string InstructorName { get; set; } = string.Empty;
    }
}
