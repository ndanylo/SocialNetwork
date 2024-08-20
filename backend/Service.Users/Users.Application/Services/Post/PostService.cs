using CSharpFunctionalExtensions;
using MassTransit;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;
using Microsoft.Extensions.Logging;
using Users.Application.Services.Abstractions;
using Users.Application.ViewModels;

namespace Users.Application.Services
{
    public class PostService : IPostService
    {
        private readonly ILogger<PostService> _logger;
        private readonly IRequestClient<GetUserPostsRequest> _requestClient;
        private readonly IRequestClient<IsPostLikedByUserRequest> _likeRequestClient;

        public PostService(IRequestClient<IsPostLikedByUserRequest> likeRequestClient,
                        IRequestClient<GetUserPostsRequest> requestClient,
                        ILogger<PostService> logger)
        {
            _requestClient = requestClient;
            _likeRequestClient = likeRequestClient;
            _logger = logger;
        }

        public async Task<Result<List<PostViewModel>>> GetPostsByUserIdAsync(Guid userId)
        {
            try
            {
                var request = new GetUserPostsRequest { UserId = userId };
                var response = await _requestClient.GetResponse<UserPostsResponse>(request);

                return Result.Success(response.Message.Posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve posts for user {UserId}", userId);
                return Result.Failure<List<PostViewModel>>("Failed to retrieve posts.");
            }
        }


        public async Task<Result<bool>> IsPostLikedByUserAsync(Guid postId, Guid userId)
        {
            try
            {
                var request = new IsPostLikedByUserRequest
                {
                    PostId = postId,
                    UserId = userId
                };

                var response = await _likeRequestClient.GetResponse<IsPostLikedByUserResponse>(request);

                return Result.Success(response.Message.IsLiked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check if post {PostId} is liked by user {UserId}", postId, userId);
                return Result.Failure<bool>("Failed to check if post is liked.");
            }
        }
    }
}