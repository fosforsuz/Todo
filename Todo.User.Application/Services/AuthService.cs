using Todo.Shared.Abstraction;
using Todo.Shared.Results;
using Todo.User.Application.Abstraction;
using Todo.User.Application.Command;
using Todo.User.Domain.Extensions;
using Todo.User.Infrastructure.Abstraction;
using Todo.User.Infrastructure.Models;

namespace Todo.User.Application.Services;

public class AuthService : IAuthService
{
    private readonly ILoginHistoryService _loginHistoryService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public AuthService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILoginHistoryService loginHistoryService,
        IRefreshTokenService refreshTokenService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _loginHistoryService = loginHistoryService ?? throw new ArgumentNullException(nameof(loginHistoryService));
        _refreshTokenService = refreshTokenService ?? throw new ArgumentNullException(nameof(refreshTokenService));
        _userRepository = unitOfWork.GetCustomRepository<IUserRepository>()
                          ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<TokenResponse>> LoginAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserForLoginAsync(command.Email, cancellationToken);
        if (user is null)
            return Result<TokenResponse>.Fail("Invalid email or password");

        var isLoginSuccessful = BCrypt.Net.BCrypt.Verify(command.Password, user.HashedPassword);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await _loginHistoryService.AddLoginHistory(
                userId: user.Id,
                ipAddress: command.IpAddress,
                userAgent: command.UserAgent,
                isSuccessful: isLoginSuccessful,
                cancellationToken
            );

            if (!isLoginSuccessful)
            {
                await SaveAndCommitAsync(cancellationToken);
                return Result<TokenResponse>.Fail("Invalid email or password");
            }

            var refreshToken = await _refreshTokenService.CreateRefreshToken(
                userId: user.Id,
                ipAddress: command.IpAddress,
                cancellationToken
            );

            await SaveAndCommitAsync(cancellationToken);

            var tokenResponse = await _tokenService.GenerateTokenAsync(
                userId: user.Id,
                username: user.Username,
                role: user.Role.GetRoleName(),
                refreshToken: refreshToken.Token,
                email: user.Email,
                refreshTokenExpires: refreshToken.ExpiresAt
            );

            return Result<TokenResponse>.Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            // TODO: Add logger here
            return Result<TokenResponse>.Fail("Unexpected error occurred during login.");
        }
    }

    private async Task SaveAndCommitAsync(CancellationToken cancellationToken)
    {
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _unitOfWork.CommitTransactionAsync(cancellationToken);
    }
}