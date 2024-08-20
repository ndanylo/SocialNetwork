using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Posts.Application.ViewModels;

namespace Posts.Application.Commands.CreatePost
{
    public class CreatePostCommand : IRequest<Result<PostViewModel>>
    {
        public string Content { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public Guid UserId { get; set; }
    }
}