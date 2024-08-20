using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Posts.Application.Services.Abstractions;
using Posts.Domain.ValueObjects;

namespace Posts.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<PhotoUrl> SaveImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("Image file is null or empty.");
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return PhotoUrl.Create($"/images/{uniqueFileName}").Value;
        }

        public byte[] GetAvatarBytes(PhotoUrl avatar)
        {
            byte[] avatarBytes = Array.Empty<byte>();

            if (!string.IsNullOrEmpty(avatar))
            {
                var avatarFilePath = Path.Combine(_environment.WebRootPath, avatar.Url.TrimStart('/'));

                try
                {
                    if (File.Exists(avatarFilePath))
                    {
                        avatarBytes = File.ReadAllBytes(avatarFilePath);
                    }
                    else
                    {
                        Console.WriteLine($"File '{avatarFilePath}' does not exist.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading file '{avatarFilePath}': {ex.Message}");
                }
            }

            return avatarBytes;
        }
    }
}
