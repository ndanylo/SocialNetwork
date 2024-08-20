using Chats.Domain.Abstractions;
using Chats.Domain.Entities;
using Chats.Domain.ValueObjects;
using Chats.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using CSharpFunctionalExtensions;

namespace Chats.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatDbContext _context;

        public ChatRepository(ChatDbContext context)
        {
            _context = context;
        }

        public Result<Chat> CreateGroupChat(ChatRoomName chatName, List<UserId> userIds)
        {
            if (userIds == null || userIds.Count < 2)
            {
                return Result.Failure<Chat>("Group chat must have at least 2 users.");
            }

            if (userIds.Distinct().Count() != userIds.Count)
            {
                return Result.Failure<Chat>("Duplicate users detected.");
            }

            var chatId = ChatId.Create(Guid.NewGuid()).Value;
            var chatUsers = new List<ChatUser>();

            foreach (var userId in userIds)
            {
                var chatUserResult = ChatUser.Create(userId, chatId);

                if (chatUserResult.IsFailure)
                {
                    return Result.Failure<Chat>($"Failed to create chat user for {userId.Id}: {chatUserResult.Error}");
                }

                chatUsers.Add(chatUserResult.Value);
            }

            var chatResult = Chat.Create(chatId, chatName, true, chatUsers);

            if (chatResult.IsFailure)
            {
                return Result.Failure<Chat>(chatResult.Error);
            }

            _context.Chats.Add(chatResult.Value);

            return chatResult;
        }

        public async Task<Result<List<Chat>>> GetUserChatsAsync(UserId userId)
        {
            try
            {
                var chats = await _context.Chats
                    .Include(chat => chat.Users)
                    .Include(chat => chat.Messages)
                    .ThenInclude(message => message.ReadBy)
                    .Where(chat => chat.Users.Any(u => u.UserId == userId))
                    .ToListAsync();

                return Result.Success(chats);
            }
            catch(Exception ex)
            {
                return Result.Failure<List<Chat>>(ex.Message);
            }
        }

        public Result<Chat> CreatePrivateChat(UserId senderId, UserId receiverId, ChatRoomName chatRoomName)
        {
            var chatId = ChatId.Create(Guid.NewGuid()).Value;

            var chatUserSenderResult = ChatUser.Create(senderId, chatId);
            var chatUserReceiverResult = ChatUser.Create(receiverId, chatId);

            if (chatUserSenderResult.IsFailure || chatUserReceiverResult.IsFailure)
            {
                throw new ArgumentException("Failed to create chat users.");
            }

            var chatUsers = new List<ChatUser>
            {
                chatUserSenderResult.Value,
                chatUserReceiverResult.Value
            };

            var createChatResult = Chat.Create(chatId, chatRoomName, false, chatUsers);

            if (createChatResult.IsFailure)
            {
                throw new ArgumentException(createChatResult.Error);
            }

            var chatRoom = createChatResult.Value;

            try
            {
                _context.Chats.Add(chatRoom);
                return Result.Success(chatRoom);
            }
            catch(Exception ex)
            {
                return Result.Failure<Chat>(ex.Message);
            }
        }

        public async Task<Result<Chat>> GetChatRoomAsync(ChatId chatRoomId, UserId userId)
        { 
            var chat = await _context.Chats
                .Include(chat => chat.Users)
                .FirstOrDefaultAsync(chat =>
                    chat.Id == chatRoomId &&
                    chat.Users.Any(u => u.UserId == userId));

            if (chat == null)
            {
                return Result.Failure<Chat>("ChatRoom not found or user is not a member.");
            }

            return Result.Success(chat);
        }

        public async Task<Result<Chat>> GetPrivateChatByUsersAsync(UserId userSenderId, UserId userReceiverId)
        {
            var chat = await _context.Chats
                .Include(chat => chat.Users)
                .Include(chat => chat.Messages)
                    .ThenInclude(message => message.ReadBy)
                .FirstOrDefaultAsync(chat =>
                    !chat.IsGroupChat &&
                    chat.Users.Any(u => u.UserId == userSenderId) &&
                    chat.Users.Any(u => u.UserId == userReceiverId));

            return chat != null ? Result.Success(chat) : Result.Failure<Chat>("Private chat not found.");
        }

        public async Task<Result> LeaveChatAsync(ChatId chatRoomId, UserId userId)
        {
            var chat = await _context.Chats
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c =>
                    c.Id == chatRoomId &&
                    c.Users.Any(u => u.UserId == userId));

            if (chat == null)
            {
                return Result.Failure("ChatRoom not found or user is not a member.");
            }

            var userInChat = chat.Users.FirstOrDefault(u => u.UserId == userId);
            if (userInChat != null)
            {
                chat.RemoveUser(userInChat);
                return Result.Success();
            }

            return Result.Failure("User not found in chat.");
        }
    }
}
