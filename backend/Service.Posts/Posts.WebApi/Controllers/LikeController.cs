using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Posts.Application.Commands.LikePost;
using Posts.Application.Commands.UnlikePost;
using Posts.Application.Queries.GetPostLikes;

namespace OnlineChat.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LikeController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        [HttpPost("like-post/{postId}")]
        public async Task<IActionResult> LikePost(Guid postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new LikePostCommand
            {
                PostId = postId,
                UserId = Guid.Parse(userId)
            };

            var likeResult = await _mediator.Send(command);

            if (likeResult.IsSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest(likeResult.Error);
            }
        }

        [HttpDelete("unlike-post/{postId}")]
        public async Task<IActionResult> UnlikePost(Guid postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new UnlikePostCommand
            {
                PostId = postId,
                UserId = Guid.Parse(userId)
            };

            var unlikeResult = await _mediator.Send(command);

            if (unlikeResult.IsSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest(unlikeResult.Error);
            }
        }


        [HttpGet("post-likes/{postId}")]
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