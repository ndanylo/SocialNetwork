using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Chats.WebApi.Requests;
using Chats.Application.Commands.CreateGroupChat;
using Chats.Application.Queries.GetMessages;
using Chats.Application.Commands.SendMessage;
using Chats.Application.Commands.LeaveChat;
using Chats.Application.Commands.ReadChatMessages;
using Chats.Application.Queries.GetChatByUser;
using Chats.Application.Queries.GetUserChats;

namespace Chat.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IMediator mediator, ILogger<ChatController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroupChat([FromBody] CreateGroupChatRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            if (request.UserIds == null || request.UserIds.Count() < 2)
            {
                return BadRequest("Not enought users for creating group chat. Group chat must contain >=3 users");
            }

            var command = new CreateGroupChatCommand
            {
                CreatorId = Guid.Parse(userId),
                Name = request.Name,
                UserIds = request.UserIds
            };

            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages(Guid chatId, int amountOfMessage, int count = 20)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var query = new GetMessagesQuery
            {
                ChatRoomId = chatId,
                AmountOfMessage = amountOfMessage,
                Count = count,
                UserId = Guid.Parse(userId)
            };

            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new SendMessageCommand
            {
                SenderId = Guid.Parse(userId),
                ChatId = request.ChatId,
                Content = request.Content
            };

            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost("leave/{chatId}")]
        public async Task<IActionResult> LeaveChat(Guid chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new LeaveChatCommand
            {
                ChatRoomId = chatId,
                UserId = Guid.Parse(userId)
            };
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpPost("read-messages/{chatId}")]
        public async Task<IActionResult> ReadChatMessages(Guid chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new ReadChatCommand
            {
                ChatRoomId = chatId,
                UserId = Guid.Parse(userId)
            };
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpGet("by-user/{receiverId}")]
        public async Task<IActionResult> GetChatByUser(Guid receiverId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var query = new GetChatByUserQuery
            {
                UserSenderId = Guid.Parse(userId),
                UserReceiverId = receiverId
            };
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("user-chats")]
        public async Task<IActionResult> GetUserChats()
        {
            _logger.LogInformation("Get User Chats was called");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var query = new GetUserChatsQuery
            {
                UserId = Guid.Parse(userId)
            };
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
