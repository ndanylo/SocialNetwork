using Authorization.Domain.Entities;

namespace Authorization.Application.Services.Abstractions
{
    public interface IJwtService
    {
        string GenerateToken(UserCredentials user);
    }
}