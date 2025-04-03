using Todo.SharedKernel.Abstraction;

namespace Todo.User.Infrastructure.Abstraction;

public interface IUserRepository : IRepository<Domain.Entity.User>
{
    Task<Domain.Entity.User?> GetUserForLoginAsync(string email, CancellationToken cancellationToken);
}