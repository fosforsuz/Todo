using Todo.Shared.Infrastructure;
using Todo.User.Infrastructure.Abstraction;
using Todo.User.Infrastructure.Data;

namespace Todo.User.Infrastructure.Persistence;

internal class UserRepository : Repository<Domain.Entity.User>, IUserRepository
{
    public UserRepository(UserDbContext dbContext) : base(dbContext)
    {
    }
}