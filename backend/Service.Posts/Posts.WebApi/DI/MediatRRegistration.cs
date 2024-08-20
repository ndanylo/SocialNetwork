using OnlineChat.Application.Comments.Commands.DeleteComment;
using Posts.Application.Commands.CreateComment;
using Posts.Application.Commands.CreatePost;
using Posts.Application.Commands.LikePost;
using Posts.Application.Commands.UnlikePost;
using Posts.Application.Queries.GetFriendsPosts;
using Posts.Application.Queries.GetPostById;
using Posts.Application.Queries.GetPostComments;
using Posts.Application.Queries.GetPostLikes;

namespace Posts.WebApi.DI
{
    public static class MediatRRegistration
    {
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateCommentCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(CreatePostCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(DeleteCommentCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(LikePostCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(UnlikePostCommand).Assembly);

                cfg.RegisterServicesFromAssembly(typeof(GetCommentsQuery).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetFriendsPostsQuery).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetPostByIdQuery).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetPostLikesQuery).Assembly);
            });

            return services;
        }
    }
}
