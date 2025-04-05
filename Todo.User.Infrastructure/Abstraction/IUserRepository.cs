using Todo.SharedKernel.Abstraction;

namespace Todo.User.Infrastructure.Abstraction;

public interface IUserRepository : IRepository<Domain.Entity.User>
{
    Task<Domain.Entity.User?> GetUserForLoginAsync(string email, CancellationToken cancellationToken);
    Task<Domain.Entity.User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Domain.Entity.User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<Domain.Entity.User?> GetUserByEmailVerificationTokenAsync(
        string emailVerificationToken,
        CancellationToken cancellationToken);
}