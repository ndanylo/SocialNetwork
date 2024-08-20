using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Moq;
using Posts.Application.Queries.GetPostsByUserId;
using Posts.Application.Services.Abstractions;
using Posts.Application.ViewModels;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;

public class GetPostsByUserIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<GetPostsByUserIdQueryHandler>> _mockLogger;
    private readonly Mock<IUserService> _mockUserService;
    private readonly GetPostsByUserIdQueryHandler _handler;

    public GetPostsByUserIdQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<GetPostsByUserIdQueryHandler>>();
        _mockUserService = new Mock<IUserService>();
        _handler = new GetPostsByUserIdQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object, _mockUserService.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetPostsByUserIdQuery { UserId = userId };

        var user = new UserViewModel { Id = userId, UserName = "TestUser" };
        var posts = new List<Post>
    {
        Post.Create(PostId.Create(Guid.NewGuid()).Value, UserId.Create(userId).Value,
            PostContent.Create("Test content 1").Value, PhotoUrl.Create("image1").Value).Value,
        Post.Create(PostId.Create(Guid.NewGuid()).Value, UserId.Create(userId).Value,
            PostContent.Create("Test content 2").Value, PhotoUrl.Create("image2").Value).Value
    };

        var postViewModels = posts.ConvertAll(p => new PostViewModel { Id = p.Id.Id, Content = p.Content });

        _mockUserService.Setup(us => us.GetUserByIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync(Result.Success(user));

        _mockUnitOfWork.Setup(uow => uow.Posts.GetPostsByUserIdsAsync(It.IsAny<List<UserId>>()))
            .ReturnsAsync(posts);

        _mockMapper.Setup(m => 
            m.Map<List<PostViewModel>>(It.IsAny<List<Post>>(), It.IsAny<Action<IMappingOperationOptions<object, List<PostViewModel>>>>()))
                .Returns(postViewModels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task Handle_InvalidUserId_ReturnsFailureResult()
    {
        // Arrange
        var query = new GetPostsByUserIdQuery { UserId = Guid.Empty };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Invalid user ID.", result.Error);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetPostsByUserIdQuery { UserId = userId };

        _mockUserService.Setup(us => us.GetUserByIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync(Result.Failure<UserViewModel>("User not found"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to retrieve user information.", result.Error);
    }

    [Fact]
    public async Task Handle_MappingException_ReturnsFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetPostsByUserIdQuery { UserId = userId };

        var user = new UserViewModel { Id = userId, UserName = "TestUser" };
        var posts = new List<Post>
        {
            Post.Create(PostId.Create(Guid.NewGuid()).Value, UserId.Create(userId).Value,
                PostContent.Create("Test content").Value, PhotoUrl.Create("image").Value).Value
        };

        _mockUserService.Setup(us => us.GetUserByIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync(Result.Success(user));

        _mockUnitOfWork.Setup(uow => uow.Posts.GetPostsByUserIdsAsync(It.IsAny<List<UserId>>()))
            .ReturnsAsync(posts);

        _mockMapper.Setup(m => m.Map<List<PostViewModel>>(It.IsAny<List<Post>>(), It.IsAny<Action<IMappingOperationOptions<object, List<PostViewModel>>>>()))
            .Throws(new AutoMapperMappingException("Mapping error"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to retrieve posts.", result.Error);
    }
}