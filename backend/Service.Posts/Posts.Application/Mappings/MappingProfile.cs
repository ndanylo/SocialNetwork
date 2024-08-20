using AutoMapper;
using Posts.Application.Services.Abstractions;
using Posts.Application.ViewModels;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;

namespace Posts.Application.Mappings
{
    public class MappingProfile : Profile
    {
        private readonly IImageService _imageService;

        public MappingProfile(IImageService imageService)
        {
            _imageService = imageService;

            CreateMap<Comment, CommentViewModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    context.Items.TryGetValue("User", out var user);
                    return (user as UserViewModel)?.FirstName + " " + (user as UserViewModel)?.LastName;
                }))
                .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    context.Items.TryGetValue("User", out var user);
                    return (user as UserViewModel)?.Avatar;
                }));

            CreateMap<Post, PostViewModel>()
                .ForMember(dest => dest.User, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    context.Items.TryGetValue("User", out var user);
                    return user as UserViewModel;
                }))
                .ForMember(dest => dest.Image, opt => opt.MapFrom((src, dest, destMember, context) =>
                    GetImageBytes(src.Image?.Url)))
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes.Count))
                .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count));

            CreateMap<Like, LikeViewModel>()
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.User, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    context.Items.TryGetValue("User", out var user);
                    return user as UserViewModel;
                }));
        }

        private byte[] GetImageBytes(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return Array.Empty<byte>();
            }
            var photoUrlResult = PhotoUrl.Create(imageUrl);
            if (photoUrlResult.IsSuccess)
            {
                return _imageService.GetAvatarBytes(photoUrlResult.Value);
            }
            return Array.Empty<byte>();
        }
    }
}
