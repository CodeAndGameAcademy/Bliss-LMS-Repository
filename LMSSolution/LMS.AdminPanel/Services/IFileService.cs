using LMS.AdminPanel.DTOs;

namespace LMS.AdminPanel.Services
{
    public interface IFileService
    {
        Task<FileUploadResultDto> UploadAsync(
            IFormFile file,
            string folder,
            string[] allowedExtensions,
            long maxSizeInBytes);

        Task<FileUploadResultDto> ReplaceAsync(
            IFormFile file,
            string existingFilePath,
            string folder,
            string[] allowedExtensions,
            long maxSizeInBytes);

        Task DeleteAsync(string filePath);
    }
}
