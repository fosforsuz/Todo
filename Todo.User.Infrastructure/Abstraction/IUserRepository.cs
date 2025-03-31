using Todo.Shared.Abstraction;
using Todo.User.Domain.Entity;

namespace Todo.User.Infrastructure.Abstraction;

public interface IUserRepository : IRepository<Domain.Entity.User>;
