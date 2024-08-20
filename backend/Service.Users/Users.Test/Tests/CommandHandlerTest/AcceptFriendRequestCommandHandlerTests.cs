using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;
using Users.Application.FriendRequests.Commands.AcceptFriendRequest;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;
using Users.Domain.Abstractions;
using Users.Infrastructure.EF;
using OnlineChat.Infrastructure.Repositories;
using Posts.Application.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Users.Application.Commands.AcceptFriendRequest;

namespace Users.Application.Tests
{
    public class AcceptFriendRequestCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly AcceptFriendRequestCommandHandler _handler;

        public AcceptFriendRequestCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _notificationServiceMock = new Mock<INotificationService>();
            _handler = new AcceptFriendRequestCommandHandler(_notificationServiceMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldAcceptFriendRequest_WhenValid()
        {
            // Arrange
            var command = new AcceptFriendRequestCommand
            {
                SenderId = Guid.NewGuid(),
                ReceiverId = Guid.NewGuid()
            };

            var senderIdResult = UserId.Create(command.SenderId).Value;
            var receiverIdResult = UserId.Create(command.ReceiverId).Value;

            var sender = User.Default;
            var receiver = User.Default;

            var friendRequestId = FriendRequestId.Create(Guid.NewGuid()).Value;
            var friendRequest = FriendRequest.Create(friendRequestId, senderIdResult, receiverIdResult, sender, receiver).Value;

            _unitOfWorkMock.Setup(u => u.FriendRequests.GetFriendRequestBySenderAndReceiverIdsAsync(senderIdResult, receiverIdResult))
                .ReturnsAsync(friendRequest);

            _unitOfWorkMock.Setup(u => u.FriendRequests.RemoveFriendRequest(friendRequest))
                .Returns(Result.Success());

            _unitOfWorkMock.Setup(u => u.Users.AddFriendAsync(receiverIdResult, senderIdResult))
                .Returns(Task.FromResult(Result.Success()));
            _unitOfWorkMock.Setup(u => u.Users.AddFriendAsync(senderIdResult, receiverIdResult))
                .Returns(Task.FromResult(Result.Success()));

            _notificationServiceMock.Setup(n => n.FriendRequestAccepted(receiverIdResult, senderIdResult))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.FriendRequests.RemoveFriendRequest(friendRequest), Times.Once);
            _unitOfWorkMock.Verify(u => u.Users.AddFriendAsync(receiverIdResult, senderIdResult), Times.Once);
            _notificationServiceMock.Verify(n => n.FriendRequestAccepted(receiverIdResult, senderIdResult), Times.Once);
        }
    }
}
