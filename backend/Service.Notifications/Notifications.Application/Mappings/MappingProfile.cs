using AutoMapper;
using Notifications.Application.ViewModels;
using Notifications.Domain.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Notification, NotificationViewModel>()
            .ForMember(dest => dest.UserSender, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                if (context.Items.TryGetValue("User", out var userObj) && userObj is UserViewModel user)
                    return user;
                return null;
            }))
            .ForMember(dest => dest.Post, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                if (context.Items.TryGetValue("Post", out var postObj) && postObj is PostViewModel post)
                    return post;
                return null;
            }))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));
    }
}