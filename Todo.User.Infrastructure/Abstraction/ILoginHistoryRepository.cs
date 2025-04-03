using Todo.SharedKernel.Abstraction;
using Todo.User.Domain.Entity;

namespace Todo.User.Infrastructure.Abstraction;

public interface ILoginHistoryRepository : IRepository<LoginHistory>;