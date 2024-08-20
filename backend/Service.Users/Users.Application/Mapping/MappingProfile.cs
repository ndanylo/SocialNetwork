using AutoMapper;
using Users.Application.Services.Abstractions;
using Users.Application.ViewModels;
using Users.Domain.Entities;

namespace Chats.Application.Mappings
{
    public class MappingProfile : Profile
    {
        private readonly IImageService _imageService;
        public MappingProfile(IImageService imageService)
        {
            _imageService = imageService;

            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => GetImageBytes(src.Avatar)));

            CreateMap<User, UserProfileViewModel>()
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => GetImageBytes(src.Avatar)))
                .ForMember(dest => dest.Posts, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items.ContainsKey("Posts") ?
                        context.Items["Posts"] as List<PostViewModel> :
                        new List<PostViewModel>()));
        }

        private byte[] GetImageBytes(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return Array.Empty<byte>();
            }
            return _imageService?.GetAvatarBytes(imageUrl) ?? Array.Empty<byte>();
        }
    }
}
