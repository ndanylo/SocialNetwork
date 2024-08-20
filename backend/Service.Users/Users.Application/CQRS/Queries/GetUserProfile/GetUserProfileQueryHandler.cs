using MediatR;
using AutoMapper;
using CSharpFunctionalExtensions;
using Users.Application.ViewModels;
using Users.Application.Services.Abstractions;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using Users.Application.Queries.GetUserProfile;
using Microsoft.Extensions.Logging;

namespace OnlineChat.Application.Users.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPostService _postService;
        private readonly ILogger<GetUserProfileQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetUserProfileQueryHandler(IMapper mapper,
                                         IUnitOfWork unitOfWork,
                                         IPostService postService,
                                         ILogger<GetUserProfileQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _postService = postService;
        }

        public async Task<Result<UserProfileViewModel>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var currentUserIdResult = UserId.Create(request.UserId);
            if (currentUserIdResult.IsFailure)
            {
                return Result.Failure<UserProfileViewModel>("Invalid user ID.");
            }

            var profileIdResult = UserId.Create(request.ProfileUserId);
            if (profileIdResult.IsFailure)
            {
                return Result.Failure<UserProfileViewModel>("Invalid profile user ID.");
            }

            var getUserResult = await _unitOfWork.Users.GetUserByIdAsync(profileIdResult.Value);
            if (getUserResult.IsFailure)
            {
                return Result.Failure<UserProfileViewModel>($"User with ID '{request.ProfileUserId}' not found.");
            }
            var user = getUserResult.Value;

            var postsResult = await _postService.GetPostsByUserIdAsync(profileIdResult.Value);
            if (postsResult.IsFailure)
            {
                return Result.Failure<UserProfileViewModel>(postsResult.Error);
            }
                
            var userProfileViewModel = _mapper.Map<UserProfileViewModel>(user, opts =>
            {
                opts.Items["Posts"] = postsResult.Value;
            });

            var getFriendsResult = await _unitOfWork.Users.GetUserFriendsAsync(profileIdResult.Value);
            if(getFriendsResult.IsFailure)
            {
                return Result.Failure<UserProfileViewModel>(getFriendsResult.Error);
            }
            userProfileViewModel.FriendsCount = getFriendsResult.Value.Count;

            if (userProfileViewModel.Posts == null)
            {
                userProfileViewModel.Posts = new List<PostViewModel>();
            }

            foreach (var postViewModel in userProfileViewModel.Posts)
            {

                var isLikedResult = await _postService.IsPostLikedByUserAsync(postViewModel.Id, currentUserIdResult.Value);
                if (isLikedResult.IsFailure)
                {
                    return Result.Failure<UserProfileViewModel>(isLikedResult.Error);
                }

                postViewModel.LikedByUser = isLikedResult.Value;
            }

            return Result.Success(userProfileViewModel);
        }
    }
}
