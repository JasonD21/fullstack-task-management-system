using Microsoft.AspNetCore.Http;
using System.IO;

namespace backend.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string subDirectory = "uploads")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded");
            }

            // Create directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath, subDirectory);
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path
            return $"/{subDirectory}/{fileName}";
        }

        public async Task<string> UploadUserAvatarAsync(IFormFile file, Guid userId)
        {
            var avatarPath = await UploadFileAsync(file, $"avatars/{userId}");
            return avatarPath;
        }

        public async Task<string> UploadTaskAttachmentAsync(IFormFile file, Guid taskId)
        {
            var attachmentPath = await UploadFileAsync(file, $"attachments/{taskId}");
            return attachmentPath;
        }

        public bool DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }

            return false;
        }
    }
}