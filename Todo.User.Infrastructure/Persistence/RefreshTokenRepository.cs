using Todo.Shared.Infrastructure;
using Todo.User.Domain.Entity;
using Todo.User.Infrastructure.Abstraction;
using Todo.User.Infrastructure.Data;

namespace Todo.User.Infrastructure.Persistence;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(UserDbContext dbContext) : base(dbContext)
    {
    }
}