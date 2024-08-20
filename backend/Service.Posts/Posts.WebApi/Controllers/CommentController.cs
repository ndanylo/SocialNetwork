using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OnlineChat.Application.Comments.Commands.DeleteComment;
using OnlineChat.WebApi.Requests;
using Posts.Application.Commands.CreateComment;
using Posts.Application.Queries.GetPostComments;

namespace OnlineChat.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateComment(CreateCommentRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new CreateCommentCommand
            {
                UserId = Guid.Parse(userId),
                Content = request.Content,
                PostId = request.PostId
            };

            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpDelete("delete/{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new DeleteCommentCommand { CommentId = commentId, UserId = Guid.Parse(userId) };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetPostComments(Guid postId)
        {
            var query = new GetCommentsQuery
            {
                PostId = postId
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
