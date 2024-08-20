using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OnlineChat.Application.FriendRequests.Commands.CancelFriendRequest;
using OnlineChat.Application.FriendRequests.Queries.GetSentFriendRequests;
using Users.Application.Commands.SendFriendRequest;
using Users.Application.Queries.GetReceivedFriendRequests;
using Users.Application.Commands.AcceptFriendRequest;
using Users.Application.Commands.DeclineFriendRequest;

namespace OnlineChat.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendRequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FriendRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("send/{receiverId}")]
        public async Task<IActionResult> SendFriendRequest(Guid receiverId)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null)
            {
                return Unauthorized();
            }
            var result = await _mediator.Send(new SendFriendRequestCommand
            {
                SenderId = Guid.Parse(senderId),
                ReceiverId = receiverId
            });

            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpGet("received")]
        public async Task<IActionResult> GetReceivedFriendRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new GetReceivedFriendRequestsQuery
            {
                UserId = Guid.Parse(userId)
            });

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("sent")]
        public async Task<IActionResult> GetSentFriendRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new GetSentFriendRequestsQuery
            {
                UserId = Guid.Parse(userId)
            });

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost("accept/{senderId}")]
        public async Task<IActionResult> AcceptFriendRequest(Guid senderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new AcceptFriendRequestCommand
            {
                ReceiverId = Guid.Parse(userId),
                SenderId = senderId
            };

            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpDelete("decline/{senderId}")]
        public async Task<IActionResult> DeclineFriendRequest(Guid senderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new DeclineFriendRequestCommand
            {
                ReceiverId = Guid.Parse(userId),
                SenderId = senderId
            };


            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpDelete("cancel/{receiverId}")]
        public async Task<IActionResult> CancelFriendRequest(Guid receiverId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new CancelFriendRequestCommand
            {
                SenderId = Guid.Parse(userId),
                ReceiverId = receiverId
            });

            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }
    }
}