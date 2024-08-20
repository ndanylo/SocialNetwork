using AutoMapper;
using Chats.Application.ViewModels;
using Chats.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace OnlineChat.Application.Mappings
{
    public class MappingProfile : Profile
    {
        private readonly ILogger<MappingProfile> _logger;

        public MappingProfile(ILogger<MappingProfile> logger)
        {
            _logger = logger;

            CreateMap<Message, MessageViewModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ChatUserId.UserId))
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.ChatId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content.Content))
                .ForMember(dest => dest.User, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var users = context.Items["Users"] as Dictionary<Guid, UserViewModel>;
                    if (users == null)
                    {
                        _logger.LogError("Users dictionary is null");
                        throw new InvalidOperationException("Users dictionary is null");
                    }

                    if (src.ChatUserId == null)
                    {
                        throw new InvalidOperationException("ChatUserId is null");
                    }

                    var userId = src.ChatUserId.UserId;
                    if (users.TryGetValue(userId, out var user))
                    {
                        return user;
                    }

                    _logger.LogError("User with ID {UserId} not found in users dictionary for message with ID: {MessageId}", userId, src.Id);
                    throw new KeyNotFoundException($"User with ID {userId} was not found in the users dictionary.");
                }))
                .ForMember(dest => dest.ReadBy, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    return src.ReadBy.Where(u => u.UserId != src.ChatUserId.UserId).ToList();
                }));

            CreateMap<Chat, ChatViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Users, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var users = context.Items["Users"] as Dictionary<Guid, UserViewModel>;
                    return src.Users.Select(u =>
                    {
                        if (users != null && users.TryGetValue(u.UserId, out var userViewModel))
                        {
                            return userViewModel;
                        }
                        return null;
                    }).Where(u => u != null).ToList();
                }))
                .ForMember(dest => dest.Messages, _ => new List<MessageViewModel>())
                .ForMember(dest => dest.UnreadMessagesCount, opt => opt.Ignore())
                .ForMember(dest => dest.LastMessage,
                    opt => opt.MapFrom(src => src.Messages.OrderByDescending(m => m.Timestamp).FirstOrDefault()))
                .AfterMap((src, dest, context) =>
                {
                    var currentUserId = (Guid)context.Items["CurrentUserId"];
                    var users = context.Items["Users"] as Dictionary<Guid, UserViewModel>;

                    if (src.IsGroupChat)
                    {
                        dest.Name = src.Name;
                    }
                    else
                    {
                        var otherUser = src.Users.FirstOrDefault(u => u.UserId != currentUserId);
                        if (otherUser != null && users != null && users.TryGetValue(otherUser.UserId, out var userViewModel))
                        {
                            dest.Name = $"{userViewModel.FirstName} {userViewModel.LastName}";
                        }
                        else
                        {
                            dest.Name = string.Empty;
                        }
                    }

                    dest.Messages = dest.Messages?.OrderBy(m => m.Timestamp).ToList();
                });

            CreateMap<ChatUser, UserViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var users = context.Items["Users"] as Dictionary<Guid, UserViewModel>;
                    return users != null && users.TryGetValue(src.UserId, out var userViewModel) ? userViewModel.UserName : string.Empty;
                }))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var users = context.Items["Users"] as Dictionary<Guid, UserViewModel>;
                    return users != null && users.TryGetValue(src.UserId, out var userViewModel) ? userViewModel.Avatar : null;
                }))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var users = context.Items["Users"] as Dictionary<Guid, UserViewModel>;
                    return users != null && users.TryGetValue(src.UserId, out var userViewModel) ? userViewModel.FirstName : string.Empty;
                }))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var users = context.Items["Users"] as Dictionary<Guid, UserViewModel>;
                    return users != null && users.TryGetValue(src.UserId, out var userViewModel) ? userViewModel.LastName : string.Empty;
                }))
                .ForMember(dest => dest.City, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var users = context.Items["Users"] as Dictionary<Guid, UserViewModel>;
                    return users != null && users.TryGetValue(src.UserId, out var userViewModel) ? userViewModel.City : string.Empty;
                }))
                .ForMember(dest => dest.Email, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var users = context.Items["Users"] as Dictionary<Guid, UserViewModel>;
                    return users != null && users.TryGetValue(src.UserId, out var userViewModel) ? userViewModel.Email : string.Empty;
                }));
        }
    }
}