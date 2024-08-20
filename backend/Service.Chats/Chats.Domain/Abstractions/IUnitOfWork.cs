using CSharpFunctionalExtensions;

namespace Chats.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IChatRepository Chats { get; }
        IMessageRepository Messages { get; }
        Task<Result> SaveChangesAsync();
    }
}
