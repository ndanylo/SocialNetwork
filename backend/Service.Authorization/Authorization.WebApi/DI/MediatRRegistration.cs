using Authorization.Application.Commands.RegisterUser;
using OnlineChat.Application.Users.Commands.LoginUser;

namespace Authorization.WebApi.DI
{
    public static class MediatRRegistration
    {
        public static void AddMediatRServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(LoginUserCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
            });
        }
    }
}
