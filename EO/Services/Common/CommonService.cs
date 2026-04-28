using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EO.Services.Common
{
    public class CommonService : ICommonService
    {
        private static readonly string[] AllowedExtensions =
        {
            ".jpg", ".jpeg", ".png", ".webp", ".gif"
        };

        public async Task<string?> SaveFileAsync(
            IFormFile file,
            string folderName,
            string userId)
        {
            if (file == null || file.Length == 0)
                return null;

            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!Array.Exists(AllowedExtensions, e => e == ext))
                throw new Exception("Only image files are allowed.");

            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var folderPath = Path.Combine(rootPath, "users", userId, folderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/users/{userId}/{folderName}/{fileName}";
        }
    }
}