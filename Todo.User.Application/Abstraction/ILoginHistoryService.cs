namespace Todo.User.Application.Abstraction;

public interface ILoginHistoryService
{
    Task AddLoginHistory(Guid userId, string? ipAddress, string? userAgent, bool isSuccessful,
        CancellationToken cancellationToken);
}