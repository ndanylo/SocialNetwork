using MassTransit;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;
using Posts.Domain.Abstractions;
using Posts.Domain.ValueObjects;

namespace Posts.Application.Consumers
{
    public class IsPostLikedByUserConsumer : IConsumer<IsPostLikedByUserRequest>
    {
        private readonly ILikeRepository _likeRepository;

        public IsPostLikedByUserConsumer(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        public async Task Consume(ConsumeContext<IsPostLikedByUserRequest> context)
        {
            var postIdResult = PostId.Create(context.Message.PostId);
            var userIdResult = UserId.Create(context.Message.UserId);

            if (postIdResult.IsFailure || userIdResult.IsFailure)
            {
                await context.RespondAsync(new IsPostLikedByUserResponse
                {
                    IsLiked = false
                });
                return;
            }

            var isLikedResult = await _likeRepository.IsPostLikedByUserAsync(postIdResult.Value, userIdResult.Value);
            if(isLikedResult.IsFailure)
            {
                await context.RespondAsync(new IsPostLikedByUserResponse
                {
                    IsLiked = false
                });
                return;
            }

            await context.RespondAsync(new IsPostLikedByUserResponse
            {
                IsLiked = isLikedResult.Value
            });
        }
    }
}
