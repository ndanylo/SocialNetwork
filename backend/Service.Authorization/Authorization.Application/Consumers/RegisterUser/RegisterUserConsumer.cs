using Authorization.Application.Commands.RegisterUser;
using MassTransit;
using MediatR;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;

namespace Authorization.Application.Consumers
{
    public class RegisterUserConsumer : IConsumer<RegisterUserRequest>
    {
        private readonly IMediator _mediator;

        public RegisterUserConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<RegisterUserRequest> context)
        {
            var request = context.Message;

            var command = new RegisterUserCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                await context.RespondAsync(new RegisterUserResponse
                {
                    UserId = result.Value,
                    Success = true
                });
            }
            else
            {
                await context.RespondAsync(new RegisterUserResponse
                {
                    Success = false
                });
            }
        }
    }
}
