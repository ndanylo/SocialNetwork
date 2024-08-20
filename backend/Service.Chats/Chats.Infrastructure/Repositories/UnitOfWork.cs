using Chats.Domain.Abstractions;
using Chats.Infrastructure.EF;
using CSharpFunctionalExtensions;

namespace Chats.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChatDbContext _context;

        public UnitOfWork(ChatDbContext context,
                        IChatRepository chatRepository,
                        IMessageRepository messageRepository)
        {
            _context = context;
            Chats = chatRepository;
            Messages = messageRepository;
        }

        public IChatRepository Chats { get; private set; }
        public IMessageRepository Messages { get; private set; }

        public async Task<Result> SaveChangesAsync()
        {
            try
            {
                return Result.Success(await _context.SaveChangesAsync());
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
