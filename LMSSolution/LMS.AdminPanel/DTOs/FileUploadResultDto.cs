namespace LMS.AdminPanel.DTOs
{
    public class FileUploadResultDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;   // relative path        
        public string BaseUrl { get; set; } = string.Empty;    
        public string FileUrl { get; set; } = string.Empty;    // absolute URL (MAUI use)
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }
}
