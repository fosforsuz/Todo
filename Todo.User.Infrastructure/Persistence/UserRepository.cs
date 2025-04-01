using Todo.Shared.Infrastructure;
using Todo.User.Infrastructure.Abstraction;
using Todo.User.Infrastructure.Data;

namespace Todo.User.Infrastructure.Persistence;

internal class UserRepository : Repository<Domain.Entity.User>, IUserRepository
{
    public UserRepository(UserDbContext context) : base(context)
    {
    }

    public async Task<Domain.Entity.User?> GetUserForLoginAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await GetSingleAsync(
            user => normalizedEmail.Equals(user.EmailLower) && user.IsActive,
            user => new Domain.Entity.User
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Username = user.Username,
                UsernameLower = user.UsernameLower,
                EmailLower = user.EmailLower,
                HashedPassword = user.HashedPassword,
                Is2FaEnabled = user.Is2FaEnabled,
                IsVerified = user.IsVerified
            },
            cancellationToken: cancellationToken
        );
    }
}