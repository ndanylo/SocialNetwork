using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Posts.Application.Services.Abstractions;
using Posts.Application.ViewModels;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;

namespace Posts.Application.Queries.GetFriendsPosts
{
    public class GetFriendsPostsQueryHandler : IRequestHandler<GetFriendsPostsQuery, Result<List<PostViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetFriendsPostsQueryHandler(IUnitOfWork unitOfWork,
                                        IMapper mapper,
                                        IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<Result<List<PostViewModel>>> Handle(GetFriendsPostsQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<List<PostViewModel>>("Invalid user ID.");
            }

            var friendIdsResult = await _userService.GetFriendIdsAsync(userIdResult.Value);
            if (friendIdsResult.IsFailure)
            {
                return Result.Failure<List<PostViewModel>>(friendIdsResult.Error);
            }

            var friendIds = friendIdsResult.Value;

            if (!friendIds.Any())
            {
                return Result.Success(new List<PostViewModel>());
            }

            var getPostsResult = await _unitOfWork.Posts.GetPostsByUserIdsAsync(friendIds);
            if(getPostsResult.IsFailure)
            {
                return Result.Failure<List<PostViewModel>>(getPostsResult.Error);
            }
            List<Post> posts = getPostsResult.Value;

            var userViewModelsResult = await _userService.GetUserListByIdAsync(friendIds.Select(x => x.Id));
            if (userViewModelsResult.IsFailure)
            {
                return Result.Failure<List<PostViewModel>>(userViewModelsResult.Error);
            }

            var userViewModels = userViewModelsResult.Value.ToDictionary(u => u.Id);

            var postViewModels = new List<PostViewModel>();

            try
            {
                foreach (var post in posts)
                {
                    if (userViewModels.TryGetValue(post.UserId, out var user))
                    {
                        var postViewModel = _mapper.Map<PostViewModel>(post, opt => opt.Items["User"] = user);

                        postViewModels.Add(postViewModel);
                    }
                }

                foreach (var postViewModel in postViewModels)
                {
                    var postIdResult = PostId.Create(postViewModel.Id);
                    if (postIdResult.IsFailure)
                    {
                        return Result.Failure<List<PostViewModel>>(postIdResult.Error);
                    }

                    var isPostLikedResult = await _unitOfWork.Posts
                        .IsPostLikedByUserAsync(postIdResult.Value, userIdResult.Value);

                    if(isPostLikedResult.IsFailure)
                    {
                        return Result.Failure<List<PostViewModel>>(isPostLikedResult.Error);
                    }

                    postViewModel.LikedByUser = isPostLikedResult.Value;
                }
            }
            catch(Exception ex)
            {
                return Result.Failure<List<PostViewModel>>(ex.Message);
            }

            return Result.Success(postViewModels);
        }

    }
}
