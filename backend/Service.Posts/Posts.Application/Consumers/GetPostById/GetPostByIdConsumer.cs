using MassTransit;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;
using MediatR;
using Posts.Application.Queries.GetPostById;
using Posts.Application.ViewModels;

namespace Posts.Application.Consumers
{
    public class GetPostByIdConsumer : IConsumer<GetPostByIdRequest>
    {
        private readonly IMediator _mediator;

        public GetPostByIdConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<GetPostByIdRequest> context)
        {
            var request = context.Message;

            var query = new GetPostByIdQuery
            {
                PostId = request.PostId,
                UserId = request.UserId
            };

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                await context.RespondAsync(new GetPostByIdResponse
                {
                    Post = result.Value
                });
            }
            else
            {
                await context.RespondAsync(new GetPostByIdResponse
                {
                    Post = new PostViewModel()
                });
            }
        }
    }
}
