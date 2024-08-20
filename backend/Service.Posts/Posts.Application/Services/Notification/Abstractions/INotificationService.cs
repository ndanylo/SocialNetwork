using CSharpFunctionalExtensions;
using Posts.Application.Services.Contracts;
using Posts.Domain.ValueObjects;

namespace Posts.Application.Services.Abstractions
{
    public interface INotificationService
    {
        Task<Result> CreateNotificationAsync(UserId userId,
                                            PostId postId,
                                            string content,
                                            NotificationType type);
    }
}
