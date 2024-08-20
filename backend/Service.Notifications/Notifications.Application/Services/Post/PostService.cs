using CSharpFunctionalExtensions;
using MassTransit;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;
using Microsoft.Extensions.Logging;
using Notifications.Application.Services.Abstraction;
using Notifications.Application.ViewModels;
using Notifications.Domain.ValueObjects;

namespace Notifications.Application.Services
{
    public class PostService : IPostService
    {
        private readonly ILogger<PostService> _logger;
        private readonly IRequestClient<GetPostByIdRequest> _requestClient;

        public PostService(IRequestClient<GetPostByIdRequest> requestClient,
                           ILogger<PostService> logger)
        {
            _requestClient = requestClient;
            _logger = logger;
        }

        public async Task<Result<PostViewModel>> GetPostByIdAsync(PostId postId, UserId userId)
        {
            try
            {
                var request = new GetPostByIdRequest
                {
                    PostId = postId.Id,
                    UserId = userId
                };
                var response = await _requestClient.GetResponse<GetPostByIdResponse>(request);

                return Result.Success(response.Message.Post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve post with ID {PostId}", postId.Id);
                return Result.Failure<PostViewModel>("Failed to retrieve post.");
            }
        }
    }
}