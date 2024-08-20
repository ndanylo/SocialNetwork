using CSharpFunctionalExtensions;
using AutoMapper;
using MediatR;
using Posts.Application.Services.Abstractions;
using Posts.Application.ViewModels;
using Posts.Domain.Abstractions;
using Posts.Domain.ValueObjects;

namespace Posts.Application.Queries.GetPostComments
{
    public class GetPostCommentsQueryHandler : IRequestHandler<GetCommentsQuery, Result<List<CommentViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetPostCommentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<Result<List<CommentViewModel>>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var postIdResult = PostId.Create(request.PostId);
            if (postIdResult.IsFailure)
            {
                return Result.Failure<List<CommentViewModel>>(postIdResult.Error);
            }

            var getCommentsResult = await _unitOfWork.Comments.GetCommentsByPostId(postIdResult.Value);
            if(getCommentsResult.IsFailure)
            {
                return Result.Failure<List<CommentViewModel>>(getCommentsResult.Error);
            }
            var comments = getCommentsResult.Value;

            if (comments == null || !comments.Any())
            {
                return Result.Success(new List<CommentViewModel>());
            }

            var userIds = comments.Select(comment => comment.UserId.Id).Distinct().ToList();

            var usersResult = await _userService.GetUserListByIdAsync(userIds);
            if (usersResult.IsFailure)
            {
                return Result.Failure<List<CommentViewModel>>(usersResult.Error);
            }

            var usersDictionary = usersResult.Value.ToDictionary(user => user.Id);

            var commentViewModels = new List<CommentViewModel>();

            try
            {
                foreach (var comment in comments)
                {
                    if (usersDictionary.TryGetValue(comment.UserId, out var user))
                    {
                        var commentViewModel = _mapper.Map<CommentViewModel>(comment, opts =>
                        {
                            opts.Items["User"] = user;
                        });

                        commentViewModels.Add(commentViewModel);
                    }
                }
                
                return Result.Success(commentViewModels);
            }
            catch(Exception ex)
            {
                return Result.Failure<List<CommentViewModel>>(ex.Message);
            }
        }
    }
}
