using Microsoft.AspNetCore.Http;
using Posts.Domain.ValueObjects;

namespace Posts.Application.Services.Abstractions
{
    public interface IImageService
    {
        Task<PhotoUrl> SaveImageAsync(IFormFile image);
        byte[] GetAvatarBytes(PhotoUrl avatar);
    }
}