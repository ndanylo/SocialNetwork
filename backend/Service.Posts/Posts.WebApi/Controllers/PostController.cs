using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Posts.Application.Commands.CreatePost;
using Posts.WebApi.Requests;
using Posts.Application.Queries.GetFriendsPosts;
using Posts.Application.Queries.GetPostById;
using Posts.Application.Queries.GetPostsByUserId;
using Posts.Application.Queries.GetPostLikes;

namespace OnlineChat.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            if (request.Image == null)
            {
                return BadRequest("Image can not be null");
            }

            var command = new CreatePostCommand
            {
                UserId = Guid.Parse(userId),
                Content = request.Content,
                Image = request.Image
            };

            var createPostResult = await _mediator.Send(command);

            if (createPostResult.IsSuccess)
            {
                return Ok(createPostResult.Value);
            }
            else
            {
                return BadRequest(createPostResult.Error);
            }
        }


        [HttpGet("friendsPost")]
        public async Task<IActionResult> GetFriendsPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var query = new GetFriendsPostsQuery
            {
                UserId = Guid.Parse(userId)
            };
            var postsResult = await _mediator.Send(query);
            if (postsResult.IsSuccess)
            {
                return Ok(postsResult.Value);
            }
            else
            {
                return BadRequest(postsResult.Error);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPostsByUserId(Guid userId)
        {
            var query = new GetPostsByUserIdQuery
            {
                UserId = userId
            };
            var postsResult = await _mediator.Send(query);

            if (postsResult.IsSuccess)
            {
                return Ok(postsResult.Value);
            }
            else
            {
                return BadRequest(postsResult.Error);
            }
        }

        [HttpGet("get/{postId}")]
        public async Task<IActionResult> GetPostById(Guid postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var query = new GetPostByIdQuery
            {
                PostId = postId,
                UserId = Guid.Parse(userId)
            };
            var postResult = await _mediator.Send(query);

            if (postResult.IsSuccess)
            {
                return Ok(postResult.Value);
            }
            else
            {
                return BadRequest(postResult.Error);
            }
        }

        [HttpGet("likes/{postId}")]
        public async Task<IActionResult> GetPostLikes(Guid postId)
        {
            var query = new GetPostLikesQuery
            {
                PostId = postId
            };
            var likesResult = await _mediator.Send(query);

            if (likesResult.IsSuccess)
            {
                return Ok(likesResult.Value);
            }
            else
            {
                return BadRequest(likesResult.Error);
            }
        }
    }
}