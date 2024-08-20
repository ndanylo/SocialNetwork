using Microsoft.AspNetCore.Http;

namespace Users.Application.Services.Abstractions
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile image);
        byte[] GetAvatarBytes(string avatar);
    }
}