using MediatR;
using CSharpFunctionalExtensions;
using Users.Application.Services.Abstractions;
using Users.Domain.ValueObjects;
using Users.Domain.Entities;
using Users.Domain.Abstractions;

namespace Users.Application.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<bool>>
    {
        private readonly IIdentityService _identityService;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(IIdentityService identityService,
                                        IImageService imageService,
                                        IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _imageService = imageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var registeredIdResult = await _identityService.RegisterUserAsync(request.Email, request.Password);
            if (registeredIdResult.IsFailure)
            {
                return Result.Failure<bool>(registeredIdResult.Error);
            }

            var userIdResult = UserId.Create(registeredIdResult.Value);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<bool>("Failed to create a UserId from the registered user.");
            }

            var user = User.Create(userIdResult.Value,
                                   request.Email,
                                   request.Email,
                                   request.FirstName,
                                   request.LastName,
                                   request.City);

            if (user.IsFailure)
            {
                return Result.Failure<bool>(user.Error);
            }

            if (request.Avatar != null)
            {
                var avatarUrl = await _imageService.SaveImageAsync(request.Avatar);
                user.Value.SetAvatar(avatarUrl);
            }

            var addUserResult = await _unitOfWork.Users.AddUserAsync(user.Value);
            if(addUserResult.IsFailure)
            {
                return Result.Failure<bool>(addUserResult.Error);
            }

            var saveChangesResult = await _unitOfWork.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<bool>(saveChangesResult.Error);
            }

            return Result.Success(true);
        }
    }
}
