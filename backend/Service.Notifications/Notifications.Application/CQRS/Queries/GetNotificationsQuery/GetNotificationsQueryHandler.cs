using AutoMapper;
using MediatR;
using CSharpFunctionalExtensions;
using Notifications.Application.ViewModels;
using Notifications.Domain.Abstractions;
using Notifications.Domain.ValueObjects;
using Notifications.Application.Queries.GetNotifications;
using Notifications.Application.Services.Abstraction;
using Microsoft.Extensions.Logging;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, Result<List<NotificationViewModel>>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserService _userService;
    private readonly IPostService _postService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetNotificationsQueryHandler> _logger;

    public GetNotificationsQueryHandler(INotificationRepository notificationRepository,
                                        ILogger<GetNotificationsQueryHandler> logger,
                                        IUserService userService,
                                        IPostService postService,
                                        IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _userService = userService;
        _postService = postService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<NotificationViewModel>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userIdResult = UserId.Create(request.UserId);
        if (userIdResult.IsFailure)
        {
            return Result.Failure<List<NotificationViewModel>>("Invalid user ID.");
        }

        var getNotificationsResult = await _notificationRepository.GetNotificationsAsync(userIdResult.Value);
        if(getNotificationsResult.IsFailure)
        {
            return Result.Failure<List<NotificationViewModel>>(getNotificationsResult.Error);
        }

        var notificationViewModels = new List<NotificationViewModel>();
        var notifications = getNotificationsResult.Value;
        try
        {
            foreach (var notification in notifications)
            {
                var getUserResult = await _userService.GetUserByIdAsync(notification.UserId);
                if (getUserResult.IsFailure)
                {
                    return Result.Failure<List<NotificationViewModel>>(getUserResult.Error);
                }
                var user = getUserResult.Value;

                var getPostResult = await _postService.GetPostByIdAsync(notification.PostId, userIdResult.Value);
                if (getPostResult.IsFailure)
                {
                    _logger.LogWarning("Post not found for notification: {NotificationId}", notification.Id);
                }
                var post = getPostResult.Value;

                var notificationViewModel = _mapper.Map<NotificationViewModel>(notification,
                    opts =>
                    {
                        opts.Items["User"] = user;
                        opts.Items["Post"] = post;
                    });

                notificationViewModels.Add(notificationViewModel);
            }
            
            return Result.Success(notificationViewModels);
        }
        catch(Exception ex)
        {
            return Result.Failure<List<NotificationViewModel>>(ex.Message);
        }
    }
}
