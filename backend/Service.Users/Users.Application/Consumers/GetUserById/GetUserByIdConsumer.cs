using MassTransit;
using MediatR;
using Users.Application.Queries.GetUsersDetails;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;
using Users.Application.ViewModels;

namespace Users.Application.Consumers
{
    public class GetUserByIdConsumer : IConsumer<GetUserByIdRequest>
    {
        private readonly IMediator _mediator;

        public GetUserByIdConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<GetUserByIdRequest> context)
        {
            var query = new GetUsersDetailsQuery
            {
                UserIds = new List<Guid>
                {
                    context.Message.UserId
                }
            };
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                var user = result.Value.FirstOrDefault();
                await context.RespondAsync(new GetUserByIdResponse
                {
                    User = user ?? new UserViewModel()
                });
            }
            else
            {
                await context.RespondAsync(new GetUserByIdResponse
                {
                    User = new UserViewModel()
                });
            }
        }
    }
}
