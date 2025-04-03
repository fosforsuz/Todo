using Todo.User.Application.Abstraction;
using Todo.User.Domain.Entity;
using Todo.User.Infrastructure.Abstraction;

namespace Todo.User.Application.Services;

public class LoginHistoryService : ILoginHistoryService
{
    private readonly ILoginHistoryRepository _loginHistoryRepository;

    public LoginHistoryService(ILoginHistoryRepository loginHistoryRepository)
    {
        _loginHistoryRepository = loginHistoryRepository ??
                                  throw new ArgumentNullException(nameof(loginHistoryRepository));
    }

    public async Task AddLoginHistory(Guid userId, string? ipAddress, string? userAgent, bool isSuccessful,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
            throw new ArgumentNullException(nameof(userId));

        var loginHistory = LoginHistory.Create(userId, ipAddress: ipAddress, userAgent: userAgent,
            isSuccessful: isSuccessful);

        await _loginHistoryRepository.AddAsync(loginHistory, cancellationToken);
    }
}