using Todo.SharedKernel.Infrastructure;
using Todo.User.Domain.Entity;
using Todo.User.Infrastructure.Abstraction;
using Todo.User.Infrastructure.Data;

namespace Todo.User.Infrastructure.Persistence;

internal class LoginHistoryRepository : Repository<LoginHistory>, ILoginHistoryRepository
{
    public LoginHistoryRepository(UserDbContext dbContext) : base(dbContext)
    {
    }
}