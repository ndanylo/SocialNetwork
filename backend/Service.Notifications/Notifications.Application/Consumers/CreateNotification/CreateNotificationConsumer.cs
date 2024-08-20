using AutoMapper;
using MassTransit;
using MediatR;
using Notifications.Application.Commands.CreateNotification;
using Notifications.Application.Hubs;
using Notifications.Application.Hubs.Abstraction;
using Notifications.Application.Services.Abstraction;
using Notifications.Application.ViewModels;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;
using Microsoft.AspNetCore.SignalR;
using Notifications.Domain.ValueObjects;
using Notifications.Domain.Abstractions;

namespace Notifications.Application.Consumers
{
    public class CreateNotificationConsumer : IConsumer<CreateNotificationRequest>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPostService _postService;
        private readonly IUserService _userService;

        public CreateNotificationConsumer(
            IHubContext<ChatHub, IChatHub> hubContext,
            INotificationRepository notificationRepository,
            IMediator mediator,
            IMapper mapper,
            IPostService postService,
            IUserService userService)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
            _mediator = mediator;
            _mapper = mapper;
            _postService = postService;
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<CreateNotificationRequest> context)
        {
            var request = context.Message;

            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                await context.RespondAsync(new CreateNotificationResponse
                {
                    Success = false,
                });
                return;
            }

            var userResult = await _userService.GetUserByIdAsync(userIdResult.Value);
            if (userResult.IsFailure)
            {
                await context.RespondAsync(new CreateNotificationResponse
                {
                    Success = false,
                });
                return;
            }

            var postIdResult = PostId.Create(request.PostId);
            if (postIdResult.IsFailure)
            {
                await context.RespondAsync(new CreateNotificationResponse
                {
                    Success = false,
                });
                return;
            }

            var postResult = await _postService.GetPostByIdAsync(postIdResult.Value, userIdResult.Value);
            if (postResult.IsFailure)
            {
                await context.RespondAsync(new CreateNotificationResponse
                {
                    Success = false
                });
                return;
            }

            var getNotificationResult = await _notificationRepository
                .GetNotificationByDetailsAsync(userIdResult.Value, postIdResult.Value, request.Type);
            
            if(getNotificationResult.IsFailure)
            {
                await context.RespondAsync(new CreateNotificationResponse
                {
                    Success = false
                });
            }
            var existingNotification = getNotificationResult.Value;

            if (existingNotification != null)
            {
                await context.RespondAsync(new CreateNotificationResponse
                {
                    Success = false
                });
                return;
            }

            var command = new CreateNotificationCommand
            {
                UserId = request.UserId,
                PostId = request.PostId,
                Content = request.Content,
                Type = request.Type
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                var notification = result.Value;

                var notificationViewModel = _mapper.Map<NotificationViewModel>(notification, opts =>
                {
                    opts.Items["User"] = userResult.Value;
                    opts.Items["Post"] = postResult.Value;
                });

                await _hubContext.Clients.User(notification.UserId.ToString())
                    .ReceiveNotification(notificationViewModel);
            }

            await context.RespondAsync(new CreateNotificationResponse
            {
                Success = result.IsSuccess
            });
        }
    }
}
