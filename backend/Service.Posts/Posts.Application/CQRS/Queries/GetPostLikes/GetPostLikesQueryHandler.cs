using AutoMapper;
using MediatR;
using CSharpFunctionalExtensions;
using Posts.Application.Services.Abstractions;
using Posts.Application.ViewModels;
using Posts.Domain.Abstractions;
using Posts.Domain.ValueObjects;
using Posts.Domain.Entities;

namespace Posts.Application.Queries.GetPostLikes
{
    public class GetPostLikesQueryHandler : IRequestHandler<GetPostLikesQuery, Result<List<LikeViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetPostLikesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<Result<List<LikeViewModel>>> Handle(GetPostLikesQuery request, CancellationToken cancellationToken)
        {
            var postIdResult = PostId.Create(request.PostId);
            if (postIdResult.IsFailure)
            {
                return Result.Failure<List<LikeViewModel>>(postIdResult.Error);
            }

            var getLikesResult = await _unitOfWork.Likes.GetLikesByPostIdAsync(postIdResult.Value);
            if(getLikesResult.IsFailure)
            {
                return Result.Failure<List<LikeViewModel>>(getLikesResult.Error);
            }

            List<Like> likes = getLikesResult.Value;

            var userIds = likes.Select(like => like.UserId.Id).Distinct().ToList();
            var usersResult = await _userService.GetUserListByIdAsync(userIds);
            if (usersResult.IsFailure)
            {
                return Result.Failure<List<LikeViewModel>>(usersResult.Error);
            }

            var usersDictionary = usersResult.Value.ToDictionary(user => user.Id);

            var likeViewModels = new List<LikeViewModel>();

            try
            {
                foreach (var like in likes)
                {
                    if (usersDictionary.TryGetValue(like.UserId, out var user))
                    {
                        var likeViewModel = _mapper.Map<LikeViewModel>(like, opts =>
                        {
                            opts.Items["User"] = user;
                        });

                        likeViewModels.Add(likeViewModel);
                    }
                }

                return Result.Success(likeViewModels);
            }
            catch(Exception ex)
            {
                return Result.Failure<List<LikeViewModel>>(ex.Message);
            }
        }
    }
}
