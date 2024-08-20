using MassTransit;
using MediatR;
using Users.Application.Queries.GetFriends;
using Users.Application.ViewModels;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;

namespace Users.Application.Consumers
{
    public class GetUserFriendsConsumer : IConsumer<GetUserFriendsRequest>
    {
        private readonly IMediator _mediator;

        public GetUserFriendsConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<GetUserFriendsRequest> context)
        {
            var query = new GetFriendsQuery
            {
                UserId = context.Message.UserId
            };
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                await context.RespondAsync(new GetUserFriendsResponse
                {
                    Friends = result.Value
                });
            }
            else
            {
                await context.RespondAsync(new GetUserFriendsResponse
                {
                    Friends = new List<UserViewModel>()
                });
            }
        }
    }
}
