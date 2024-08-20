using MassTransit;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;
using MediatR;
using Users.Application.Queries.GetUsersDetails;
using CSharpFunctionalExtensions;
using Users.Application.ViewModels;

namespace Users.Application.Consumers
{
    public class GetUserListByIdConsumer : IConsumer<GetUserListByIdRequest>
    {
        private readonly IMediator _mediator;

        public GetUserListByIdConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<GetUserListByIdRequest> context)
        {
            var request = context.Message;

            var query = new GetUsersDetailsQuery
            {
                UserIds = request.UserIds.ToList()
            };

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                await context.RespondAsync(new GetUserListByIdResponse
                {
                    Users = result.Value
                });
            }
            else
            {
                await context.RespondAsync(new GetUserListByIdResponse
                {
                    Users = new List<UserViewModel>()
                });
            }
        }
    }
}
