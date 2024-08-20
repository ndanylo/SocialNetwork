using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notifications.Application.Commands.DeleteAllNotifications;
using Notifications.Application.Commands.DeleteNotification;
using Notifications.Application.Queries.GetNotifications;
using System.Security.Claims;

namespace Notifications.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim ?? throw new InvalidOperationException("User ID not found in token."));
        }

        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            var userId = GetUserIdFromToken();
            var command = new DeleteAllNotificationsCommand
            {
                UserId = userId
            };
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.Error);
        }

        [HttpDelete("delete/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(Guid notificationId)
        {
            var userId = GetUserIdFromToken();
            var command = new DeleteNotificationCommand
            {
                NotificationId = notificationId,
                UserId = userId
            };
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.Error);
        }

        [HttpGet("get-all-notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = GetUserIdFromToken();
            var query = new GetNotificationsQuery
            {
                UserId = userId
            };
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(result.Error);
        }
    }
}
