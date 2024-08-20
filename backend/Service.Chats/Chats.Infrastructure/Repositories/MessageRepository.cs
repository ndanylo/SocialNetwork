using Chats.Domain.Abstractions;
using Chats.Domain.Entities;
using Chats.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Chats.Infrastructure.EF;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Chats.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(ChatDbContext context, ILogger<MessageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<Message>>> ReadChatMessagesAsync(ChatId chatId, UserId userId)
        {
            var chat = await _context.Chats
                .Include(c => c.Users)
                .Include(c => c.Messages)
                .ThenInclude(m => m.ReadBy)
                .FirstOrDefaultAsync(c => c.Id == chatId &&
                    c.Users.Any(u => u.UserId == userId));

            if (chat == null)
            {
                return Result.Failure<List<Message>>("Chat not found or user is not a member.");
            }

            var chatUser = chat.Users.FirstOrDefault(u => u.UserId == userId);
            if (chatUser == null)
            {
                return Result.Failure<List<Message>>("User is not a member of this chat.");
            }

            var unreadMessages = chat.Messages
                .Where(m => !m.ReadBy.Contains(chatUser)
                    && m.ChatUserId != chatUser.Id).ToList();

            foreach (var message in unreadMessages)
            {
                message.MarkAsReadBy(chatUser);
            }

            return Result.Success(unreadMessages);
        }

        public async Task<Result<int>> GetUnreadMessageCountForUserAsync(UserId userId, ChatId chatId)
        {
            var chatUser = await _context.ChatUsers
                .FirstOrDefaultAsync(cu => cu.UserId == userId && cu.ChatId == chatId);

            if (chatUser == null)
            {
                return Result.Failure<int>("User not found in the specified chat.");
            }

            var unreadMessageCount = await _context.Messages
                .Include(m => m.ReadBy)
                .Where(m => m.ChatId == chatId)
                .CountAsync(m => !m.ReadBy.Any(cu => cu.Id == chatUser.Id) && m.ChatUserId != chatUser.Id);

            return Result.Success(unreadMessageCount);
        }

        public async Task<Result<List<Message>>> GetMessagesAsync(ChatId chatId, int skip, int take, UserId userId)
        {
            var chat = await _context.Chats
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == chatId && c.Users.Any(u => u.UserId == userId));

            if (chat == null)
            {
                return Result.Failure<List<Message>>("Chat not found or user is not a member.");
            }

            var messages = await _context.Messages
                            .Include(m => m.ReadBy)
                            .Where(m => m.ChatId == chatId)
                            .OrderByDescending(m => m.Timestamp)
                            .Skip(skip)
                            .Take(take)
                            .OrderBy(m => m.Timestamp)
                            .ToListAsync();

            return Result.Success(messages);
        }

        public async Task<Result<Message>> GetLastMessageAsync(ChatId chatId)
        {
            var lastMessage = await _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.Timestamp)
                .FirstOrDefaultAsync();

            if (lastMessage == null)
            {
                return Result.Failure<Message>("No messages found in the chat.");
            }

            return Result.Success(lastMessage);
        }

        public async Task<Result<Message>> SendMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            return Result.Success(message);
        }
    }
}