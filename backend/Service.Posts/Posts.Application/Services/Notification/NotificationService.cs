using CSharpFunctionalExtensions;
using MassTransit;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;
using Microsoft.Extensions.Logging;
using Posts.Application.Services.Abstractions;
using Posts.Application.Services.Contracts;
using Posts.Domain.ValueObjects;

namespace Posts.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRequestClient<CreateNotificationRequest> _requestClient;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IRequestClient<CreateNotificationRequest> requestClient,
                                ILogger<NotificationService> logger)
        {
            _requestClient = requestClient;
            _logger = logger;
        }

        public async Task<Result> CreateNotificationAsync(UserId userId,
                                                        PostId postId,
                                                        string content,
                                                        NotificationType type)
        {
            try
            {
                var request = new CreateNotificationRequest
                {
                    UserId = userId,
                    PostId = postId,
                    Content = content,
                    Type = type
                };

                var response = await _requestClient.GetResponse<CreateNotificationResponse>(request);

                if (response.Message.Success)
                {
                    _logger.LogInformation("Notification created successfully for UserId: {UserId} and PostId: {PostId}", userId.Id, postId.Id);
                    return Result.Success();
                }
                else
                {
                    _logger.LogWarning("Failed to create notification for UserId: {UserId} and PostId: {PostId}", userId.Id, postId.Id);
                    return Result.Failure("Failed to create notification.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification for UserId: {UserId} and PostId: {PostId}", userId.Id, postId.Id);
                return Result.Failure("Error while creating notification.");
            }
        }
    }
}
