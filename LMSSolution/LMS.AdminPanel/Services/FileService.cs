using LMS.AdminPanel.DTOs;
using LMS.AdminPanel.Exceptions;

namespace LMS.AdminPanel.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _http;

        public FileService(IWebHostEnvironment env, IHttpContextAccessor http)
        {
            _env = env;
            _http = http;
        }

        private void ValidateFile(IFormFile file, string[] allowedExtensions, long maxSize)
        {
            if (file == null || file.Length == 0)
                throw new FileValidationException("File is required");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                throw new FileValidationException("Invalid file type");

            if (file.Length > maxSize)
                throw new FileValidationException("File too large");
        }

        public async Task<FileUploadResultDto> UploadAsync(IFormFile file, string folder, string[] allowedExtensions, long maxSize)
        {
            ValidateFile(file, allowedExtensions, maxSize);

            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", folder);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var request = _http.HttpContext!.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return new FileUploadResultDto
            {
                FileName = fileName,
                FilePath = $"uploads/{folder}/{fileName}",
                FileUrl = $"{baseUrl}/uploads/{folder}/{fileName}",
                FileSize = file.Length,
                ContentType = file.ContentType
            };
        }

        public async Task<FileUploadResultDto> ReplaceAsync(IFormFile file, string existingFilePath, string folder, string[] allowedExtensions, long maxSize)
        {
            var result = await UploadAsync(file, folder, allowedExtensions, maxSize);
            await DeleteAsync(existingFilePath);
            return result;
        }

        public Task DeleteAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return Task.CompletedTask;

            var fullPath = Path.Combine(_env.WebRootPath, filePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }
    }
}
