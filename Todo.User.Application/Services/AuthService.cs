using Todo.Shared.Contracts.Constant;
using Todo.SharedKernel.Abstraction;
using Todo.SharedKernel.Enums;
using Todo.SharedKernel.Extensions;
using Todo.SharedKernel.Factory;
using Todo.SharedKernel.Logger;
using Todo.SharedKernel.Messaging;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;
using Todo.User.Application.Command;
using Todo.User.Infrastructure.Abstraction;
using Todo.User.Infrastructure.Models;

namespace Todo.User.Application.Services;

public class AuthService : IAuthService
{
    private readonly ILoggerService<AuthService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ILoginHistoryService _loginHistoryService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IEmailFactory _emailFactory;
    private readonly IRabbitMqEmailPublisher _rabbitMqEmailPublisher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILoginHistoryService loginHistoryService,
        IRefreshTokenService refreshTokenService, ILoggerService<AuthService> logger, IEmailFactory emailFactory,
        IRabbitMqEmailPublisher rabbitMqEmailPublisher)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _loginHistoryService = loginHistoryService ?? throw new ArgumentNullException(nameof(loginHistoryService));
        _refreshTokenService = refreshTokenService ?? throw new ArgumentNullException(nameof(refreshTokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailFactory = emailFactory ?? throw new ArgumentNullException(nameof(emailFactory));
        _rabbitMqEmailPublisher =
            rabbitMqEmailPublisher ?? throw new ArgumentNullException(nameof(rabbitMqEmailPublisher));
        _userRepository = unitOfWork.GetCustomRepository<IUserRepository>()
                          ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<TokenResponse>> LoginAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserForLoginAsync(command.Email, cancellationToken);
        if (user is null)
            return Result<TokenResponse>.Fail("Invalid email or password");

        var isLoginSuccessful = BCrypt.Net.BCrypt.Verify(command.Password, user.HashedPassword);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await _loginHistoryService.AddLoginHistory(
                user.Id,
                command.IpAddress,
                command.UserAgent,
                isLoginSuccessful,
                cancellationToken
            );

            if (!isLoginSuccessful)
            {
                await SaveAndCommitAsync(cancellationToken);
                return Result<TokenResponse>.Fail("Invalid email or password");
            }

            var refreshToken = await _refreshTokenService.CreateRefreshToken(
                user.Id,
                command.IpAddress,
                cancellationToken
            );

            await SaveAndCommitAsync(cancellationToken);

            var tokenResponse = await _tokenService.GenerateTokenAsync(
                user.Id,
                username: user.Username,
                role: user.Role.GetRoleName(),
                refreshToken: refreshToken.Token,
                email: user.Email,
                refreshTokenExpires: refreshToken.ExpiresAt
            );

            await _logger.LogInformationAsync($"User who has Id: {user.Id} logged in successfully", cancellationToken);

            return Result<TokenResponse>.Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            await _unitOfWork.SafeRollbackAsync(cancellationToken);
            await _logger.LogCriticalAsync("Error occurred during login", ex, cancellationToken);
            return Result<TokenResponse>.Fail("Unexpected error occurred during login.");
        }
    }

    public async Task<Result<CommandResponse>> SendVerifyMailAsync(SendVerifyMailCommand command,
        CancellationToken cancellationToken)
    {
        var user = await GetUserByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result<CommandResponse>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound);

        user.CreateVerificationToken();

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            await _userRepository.UpdateAsync(user, cancellationToken);

            var email = await _emailFactory.CreateAsync(
                type: EmailEventType.EmailConfirmation,
                to: user.Email,
                metadata: new Dictionary<string, string?>()
                {
                    { "name", user.Name },
                    { "verificationToken", user.EmailVerificationToken },
                    { "verificationTokenExpiresAt", user.EmailVerificationTokenExpiresAt.ToString() }
                }
            );

            await _rabbitMqEmailPublisher.PublishEmailEventAsync(email, RabbitMqQueues.EmailQueue, cancellationToken);

            await SaveAndCommitAsync(cancellationToken);
            await _logger.LogInformationAsync($"Verification email sent to user with Id: {user.Id}", cancellationToken);

            return Result<CommandResponse>.Ok(CreateSuccessResponse(user));
        }
        catch (Exception e)
        {
            await _unitOfWork.SafeRollbackAsync(cancellationToken);
            await _logger.LogCriticalAsync("Error occurred during email verification", e, cancellationToken);
            return Result<CommandResponse>.Fail("Unexpected error occurred during email verification.");
        }
    }

    public async Task<Result<CommandResponse>> VerifyEmailAsync(VerifyMailCommand command,
        CancellationToken cancellationToken)
    {
        var user = await GetUserByEmailVerificationTokenAsync(command.VerifyToken, cancellationToken);
        if (user is null)
            return Result<CommandResponse>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound);

        if (user.IsEmailVerified)
            return Result<CommandResponse>.Fail(ErrorMessages.Expired.EmailAlreadyVerified,
                ErrorCodes.EmailAlreadyVerified);

        if (user.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
            return Result<CommandResponse>.Fail(ErrorMessages.Expired.EmailVerificationToken,
                ErrorCodes.EmailVerificationTokenExpired);

        user.VerifyEmail();
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            await _userRepository.UpdateAsync(user, cancellationToken);

            await SaveAndCommitAsync(cancellationToken);
            await _logger.LogInformationAsync($"Email verified for user with Id: {user.Id}", cancellationToken);

            return Result<CommandResponse>.Ok(CreateSuccessResponse(user));
        }
        catch (Exception e)
        {
            await _unitOfWork.SafeRollbackAsync(cancellationToken);
            await _logger.LogCriticalAsync("Error occurred during email verification", e, cancellationToken);
            return Result<CommandResponse>.Fail("Unexpected error occurred during email verification.");
        }
    }

    private async Task<Domain.Entity.User?> GetUserByEmailVerificationTokenAsync(
        string emailVerificationToken,
        CancellationToken cancellationToken)
    {
        return await _userRepository.GetSingleAsync(
            predicate: user => user.EmailVerificationToken == emailVerificationToken && user.IsActive,
            selector: user => new Domain.Entity.User
            {
                Name = user.Name,
                Username = user.Username,
                UsernameLower = user.UsernameLower,
                Email = user.Email,
                EmailLower = user.EmailLower,
                HashedPassword = string.Empty,
                IsEmailVerified = user.IsEmailVerified,
                EmailVerificationToken = user.EmailVerificationToken,
                EmailVerificationTokenExpiresAt = user.EmailVerificationTokenExpiresAt,
            },
            cancellationToken: cancellationToken
        );
    }

    private async Task<Domain.Entity.User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _userRepository.GetSingleAsync(
            predicate: user => user.Id == userId && user.IsActive,
            selector: user => new Domain.Entity.User
            {
                Name = user.Name,
                Username = user.Username,
                UsernameLower = user.UsernameLower,
                Email = user.Email,
                EmailLower = user.EmailLower,
                HashedPassword = string.Empty,
                EmailVerificationToken = user.EmailVerificationToken,
                EmailVerificationTokenExpiresAt = user.EmailVerificationTokenExpiresAt,
            },
            cancellationToken: cancellationToken
        );
    }

    private async Task SaveAndCommitAsync(CancellationToken cancellationToken)
    {
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _unitOfWork.CommitTransactionAsync(cancellationToken);
    }

    private static CommandResponse CreateSuccessResponse(Domain.Entity.User user)
    {
        return new CommandResponse(user.CreatedAt, string.Empty, user.Id);
    }
}