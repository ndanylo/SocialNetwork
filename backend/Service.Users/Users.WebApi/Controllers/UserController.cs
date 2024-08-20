using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Users.Application.Queries.GetFriends;
using Users.Application.Commands.RemoveFriend;
using Users.Application.Queries.GetAllUsers;
using Users.Application.Queries.GetUserProfile;
using Users.Application.Queries.GetUsersDetails;
using Users.WebApi.Requests;
using Users.Application.Commands.CreateUser;

namespace OnlineChat.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{userId}/friends")]
        public async Task<IActionResult> GetFriends(Guid userId)
        {
            _logger.LogInformation("Getting friends for user ID: {UserId}", userId);

            var query = new GetFriendsQuery
            {
                UserId = userId
            };
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to get friends for user ID: {UserId}. Error: {Error}", userId, result.Error);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully retrieved friends for user ID: {UserId}", userId);
            return Ok(result.Value);
        }

        [HttpPost("removeFriend/{friendId}")]
        public async Task<IActionResult> RemoveFriend(Guid friendId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("RemoveFriend request made by an unauthorized user.");
                return Unauthorized("User is not authenticated.");
            }

            _logger.LogInformation("User ID: {UserId} requested to remove friend ID: {FriendId}", userId, friendId);

            var command = new RemoveFriendCommand
            {
                UserId = Guid.Parse(userId),
                FriendId = friendId
            };
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to remove friend ID: {FriendId} for user ID: {UserId}. Error: {Error}", friendId, userId, result.Error);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully removed friend ID: {FriendId} for user ID: {UserId}", friendId, userId);
            return NoContent();
        }

        [HttpGet("users-detail")]
        public async Task<IActionResult> GetUsersDetails(List<Guid> userIds)
        {
            _logger.LogInformation("Getting user details for IDs: {UserIds}", string.Join(", ", userIds));

            var query = new GetUsersDetailsQuery
            {
                UserIds = userIds
            };
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to get user details. Error: {Error}", result.Error);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully retrieved user details for IDs: {UserIds}", string.Join(", ", userIds));
            return Ok(result.Value);
        }

        [HttpGet("userDetail/{userId}")]
        public async Task<IActionResult> GetPostsByUserId(Guid userId)
        {
            _logger.LogInformation("Getting user details for user ID: {UserId}", userId);

            var query = new GetUsersDetailsQuery
            {
                UserIds = new List<Guid> { userId }
            };
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to get user details for user ID: {UserId}. Error: {Error}", userId, result.Error);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully retrieved user details for user ID: {UserId}", userId);
            return Ok(result.Value.FirstOrDefault());
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                _logger.LogWarning("GetAllUsers request made by an unauthorized user.");
                return Unauthorized("UserId claim is missing.");
            }

            var userId = userIdClaim.Value;
            _logger.LogInformation("Getting all users for the current user ID: {UserId}", userId);

            var query = new GetAllUsersQuery
            {
                UserId = Guid.Parse(userId)
            };
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to get all users for user ID: {UserId}. Error: {Error}", userId, result.Error);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully retrieved all users for user ID: {UserId}", userId);
            return Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            _logger.LogInformation("Creating user with email: {Email}", request.Email);

            var command = new CreateUserCommand
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                City = request.City,
                Avatar = request.Avatar
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to create user with email: {Email}. Error: {Error}", request.Email, result.Error);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully created user with email: {Email}", request.Email);
            return Ok(result.Value);
        }

        [HttpGet("profile/{profileUserId}")]
        public async Task<IActionResult> GetUserProfile(Guid profileUserId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("GetUserProfile request made by an unauthorized user.");
                return Unauthorized("User is not authenticated.");
            }

            _logger.LogInformation("Getting profile for user ID: {UserId} requested by user ID: {RequesterId}", profileUserId, userId);

            var query = new GetUserProfileQuery
            {
                ProfileUserId = profileUserId,
                UserId = Guid.Parse(userId)
            };

            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to get profile for user ID: {UserId}. Error: {Error}", profileUserId, result.Error);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully retrieved profile for user ID: {UserId}", profileUserId);
            return Ok(result.Value);
        }
    }
}
