namespace LMS.StudentAPI.Constants
{
    public static class FileConstants
    {
        public static class Defaults
        {            
            public const string UserImage = "uploads/default/user.png";         
        }

        public static class Image
        {
            public static readonly string[] AllowedExtensions =
            {
                ".jpg", ".jpeg", ".png", ".webp"
            };

            public const long MaxSize = 2 * 1024 * 1024; // 2 MB
        }

        public static class Document
        {
            public static readonly string[] AllowedExtensions =
            {
                ".pdf", ".docx"
            };

            public const long MaxSize = 10 * 1024 * 1024; // 10 MB
        }

        public static class Video
        {
            public static readonly string[] AllowedExtensions =
            {
                ".mp4", ".mov"
            };

            public const long MaxSize = 100 * 1024 * 1024; // 100 MB
        }
    }
}
