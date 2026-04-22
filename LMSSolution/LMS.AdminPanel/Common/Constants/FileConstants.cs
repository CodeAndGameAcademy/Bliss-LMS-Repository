namespace LMS.AdminPanel.Common.Constants
{
    public static class FileConstants
    {
        public static class Defaults
        {
            public const string CategoryImage = "uploads/default/category.png";
            public const string InstructorImage = "uploads/default/instructor.png";
            public const string UserImage = "uploads/default/user.png";
            public const string AdminImage = "uploads/default/admin.png";
            public const string CourseThumbnailImage = "uploads/default/course_thumbnail.png";
        }

        public static class Image
        {
            public static readonly string[] AllowedExtensions =
            {
                ".jpg", ".jpeg", ".png", ".webp"
            };

            public const long MaxSize = 2 * 1024 * 1024;
        }

        public static class Document
        {
            public static readonly string[] AllowedExtensions =
            {
                ".pdf", ".docx"
            };

            public const long MaxSize = 10 * 1024 * 1024;
        }

        public static class Video
        {
            public static readonly string[] AllowedExtensions =
            {
                ".mp4", ".mov"
            };

            public const long MaxSize = 100 * 1024 * 1024;
        }
    }
}
