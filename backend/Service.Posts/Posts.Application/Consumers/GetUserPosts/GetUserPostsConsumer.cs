using MassTransit;
using Posts.Application.Queries.GetPostsByUserId;
using MediatR;
using Microsoft.Extensions.Logging;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;

namespace Posts.Application.Consumers
{
    public class GetUserPostsConsumer : IConsumer<GetUserPostsRequest>
    {
        private readonly IMediator _mediator;

        private readonly ILogger<GetPostsByUserIdQueryHandler> _logger;

        public GetUserPostsConsumer(IMediator mediator, ILogger<GetPostsByUserIdQueryHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetUserPostsRequest> context)
        {
            _logger.LogInformation("Perfoming GetUserPostsConsumer");

            var query = new GetPostsByUserIdQuery
            {
                UserId = context.Message.UserId
            };
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                await context.RespondAsync(
                    new UserPostsResponse
                    {
                        Posts = result.Value
                    }
                );
            }
            else
            {
                throw new Exception(result.Error);
            }
        }
    }
}