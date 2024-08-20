using Microsoft.EntityFrameworkCore;
using Notifications.Domain.Abstractions;
using Notifications.Domain.Entities;
using Notifications.Infrastructure.EF;
using Notifications.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Notifications.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Notification?>> GetNotificationByDetailsAsync(UserId userId, PostId postId, NotificationType type)
        {
            try
            {
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.UserId == userId && n.PostId == postId && n.Type == type);

                return Result.Success(notification);
            }
            catch(Exception ex)
            {
                return Result.Failure<Notification?>(ex.Message);
            }
        }

        public async Task<Result> AddNotificationAsync(Notification notification)
        {
            if (notification == null)
            {
                return Result.Failure("Notification is null");
            }

            try
            {
                await _context.Notifications.AddAsync(notification);
                return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> RemoveNotificationAsync(NotificationId notificationId, UserId userId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                if (notification.UserId == userId)
                {
                    _context.Notifications.Remove(notification);
                    return Result.Success();
                }
            }
            return Result.Failure("notificaiton is not found");
        }

        public async Task<Result> RemoveAllNotificationsAsync(UserId userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .ToListAsync();

                if (notifications.Any())
                {
                    _context.Notifications.RemoveRange(notifications);
                }

                return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<IEnumerable<Notification>>> GetNotificationsAsync(UserId userId)
        {
            try
            {
                var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

                return Result.Success(notifications.AsEnumerable());
            }
            catch(Exception ex)
            {
                return Result.Failure<IEnumerable<Notification>>(ex.Message);
            }
        }

        public async Task<Result<Notification>> GetNotificationByIdAsync(NotificationId notificationId, UserId userId)
        {
            try
            {
                var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

                if (notification == null)
                {
                    return Result.Failure<Notification>($"Notification with ID {notificationId} not found for user with ID {userId}");
                }

                return Result.Success(notification);
            }
            catch(Exception ex)
            {
                return Result.Failure<Notification>(ex.Message);
            }
        }

        public async Task<Result> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
