using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Moq;
using Notifications.Application.Queries.GetNotifications;
using Notifications.Application.Services.Abstraction;
using Notifications.Application.ViewModels;
using Notifications.Domain.Abstractions;
using Notifications.Domain.Entities;
using Notifications.Domain.ValueObjects;

namespace Notifications.Tests.Application.Queries
{
    public class GetNotificationsQueryHandlerTests
    {
        private readonly Mock<INotificationRepository> _mockNotificationRepository;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IPostService> _mockPostService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<GetNotificationsQueryHandler>> _mockLogger;
        private readonly GetNotificationsQueryHandler _handler;

        public GetNotificationsQueryHandlerTests()
        {
            _mockNotificationRepository = new Mock<INotificationRepository>();
            _mockUserService = new Mock<IUserService>();
            _mockPostService = new Mock<IPostService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<GetNotificationsQueryHandler>>();

            _handler = new GetNotificationsQueryHandler(_mockNotificationRepository.Object,
                                                        _mockLogger.Object,
                                                        _mockUserService.Object,
                                                        _mockPostService.Object,
                                                        _mockMapper.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetNotificationsQuery { UserId = userId };
            var userIdValueObject = UserId.Create(userId).Value;

            var notifications = new List<Notification>
            {
                CreateMockNotification(userIdValueObject),
                CreateMockNotification(userIdValueObject)
            };

            _mockNotificationRepository.Setup(r => r.GetNotificationsAsync(It.IsAny<UserId>()))
                .ReturnsAsync(notifications);

            _mockUserService.Setup(s => s.GetUserByIdAsync(It.IsAny<UserId>()))
                .ReturnsAsync(Result.Success(new UserViewModel()));

            _mockPostService.Setup(s => s.GetPostByIdAsync(It.IsAny<PostId>(), It.IsAny<UserId>()))
                .ReturnsAsync(Result.Success(new PostViewModel()));

            _mockMapper.Setup(m => m.Map<NotificationViewModel>(It.IsAny<Notification>(), (Action<IMappingOperationOptions<object, NotificationViewModel>>)It.IsAny<Action<IMappingOperationOptions<Notification, NotificationViewModel>>>()))
                .Returns(new NotificationViewModel());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
        }

        [Fact]
        public async Task Handle_InvalidUserId_ReturnsFailureResult()
        {
            // Arrange
            var query = new GetNotificationsQuery { UserId = Guid.Empty };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Invalid user ID", result.Error);
        }

        private Notification CreateMockNotification(UserId userId)
        {
            var notificationId = NotificationId.Create(Guid.NewGuid()).Value;
            var postId = PostId.Create(Guid.NewGuid()).Value;
            var content = NotificationContent.Create("Test content").Value;
            return Notification.Create(notificationId, userId, postId, content, NotificationType.Like).Value;
        }
    }
}