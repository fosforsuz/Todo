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

public class AuthService : BaseService<AuthService>, IAuthService
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
        IRabbitMqEmailPublisher rabbitMqEmailPublisher) : base(unitOfWork, logger)
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
            return Result<TokenResponse>.Fail(ErrorMessages.Invalid.EmailOrPassword, ErrorCodes.InvalidEmailOrPassword);

        var isLoginSuccessful = BCrypt.Net.BCrypt.Verify(command.Password, user.HashedPassword);

        if (isLoginSuccessful && user.Is2FaEnabled)
            return await SendOtpAsync(user, cancellationToken);

        return await PerformLoginTransactionAsync(user: user, isLoginSuccessful: isLoginSuccessful,
            ipAddress: command.IpAddress, userAgent: command.UserAgent, cancellationToken);
    }

    public async Task<Result<TokenResponse>> VerifyOtpAsync(VerifyOtpCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserForOtpLoginAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result<TokenResponse>.Fail(ErrorMessages.Invalid.Otp, ErrorCodes.InvalidOtp);

        if (!user.Is2FaEnabled)
            return Result<TokenResponse>.Fail(ErrorMessages.NotEnabled.TwoFactorAuthentication,
                ErrorCodes.TwoFactorAuthenticationNotEnabled);

        if (user.OtpCodeExpiresAt < DateTime.UtcNow)
            return Result<TokenResponse>.Fail(ErrorMessages.Expired.Otp, ErrorCodes.OtpExpired);

        if (user.OtpTryCount >= 3)
            return Result<TokenResponse>.Fail(ErrorMessages.Blocked.OtpTryExceeded, ErrorCodes.OtpTryExceeded);

        var isOtpValid = !string.IsNullOrEmpty(user.OtpCode) && user.OtpCode == command.Otp;

        if (!isOtpValid)
        {
            user.IncreaseOtpTryCount();
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        return await PerformLoginTransactionAsync(
            user: user,
            isLoginSuccessful: isOtpValid,
            ipAddress: command.IpAddress,
            userAgent: command.UserAgent,
            cancellationToken: cancellationToken
        );
    }

    public async Task<Result<CommandResponse>> SendVerifyMailAsync(SendVerifyMailCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result<CommandResponse>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound);


        var result = await ExecuteCommandAsync(
            command: command,
            action: async () =>
            {
                user.CreateVerificationToken();
                await _userRepository.UpdateAsync(user, cancellationToken);

                var email = await _emailFactory.CreateAsync(
                    type: EmailEventType.EmailConfirmation,
                    to: user.Email,
                    metadata: new Dictionary<string, string?>
                    {
                        { "name", user.Name },
                        { "verificationToken", user.EmailVerificationToken },
                        { "verificationTokenExpiresAt", user.EmailVerificationTokenExpiresAt.ToString() }
                    }
                );

                await _rabbitMqEmailPublisher.PublishEmailEventAsync(email, RabbitMqQueues.EmailQueue,
                    cancellationToken);

                return Result<CommandResponse>.Ok(Success(user.UpdatedAt, correlationId: user.Id));
            },
            onFailure: async (_, res) =>
            {
                var errorMessage = string.Join(", ", res.GetErrors());
                await _logger.LogWarningAsync($"Command failed: {errorMessage}", cancellationToken);
            },
            onSuccess: async (_, _) =>
            {
                await _logger.LogInformationAsync(
                    $"Command {nameof(SendPasswordResetMailCommand)} executed successfully for UserId: {user.Id}",
                    cancellationToken);
            },
            onError: async (mailCommand, exception) =>
            {
                await _logger.LogByExceptionSeverityAsync(
                    $"Error occurred during email verification: {mailCommand.UserId}",
                    exception,
                    cancellationToken);
            },
            cancellationToken: cancellationToken
        );

        return result;
    }

    public async Task<Result<CommandResponse>> VerifyEmailAsync(VerifyMailCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailVerificationTokenAsync(command.VerifyToken, cancellationToken);
        if (user is null)
            return Result<CommandResponse>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound);

        if (user.IsEmailVerified)
            return Result<CommandResponse>.Fail(ErrorMessages.Verified.Email,
                ErrorCodes.EmailAlreadyVerified);

        if (user.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
            return Result<CommandResponse>.Fail(ErrorMessages.Expired.EmailVerificationToken,
                ErrorCodes.EmailVerificationTokenExpired);

        var result = await ExecuteCommandAsync(
            command,
            action: async () =>
            {
                user.VerifyEmail();
                await _userRepository.UpdateAsync(user, cancellationToken);
                return Result<CommandResponse>.Ok(Success(user.UpdatedAt, correlationId: user.Id));
            },
            onFailure: async (_, res) =>
            {
                var errorMessage = string.Join(", ", res.GetErrors());
                await _logger.LogWarningAsync($"Command failed: {errorMessage}", cancellationToken);
            },
            onSuccess: async (_, _) =>
            {
                await _logger.LogInformationAsync(
                    $"Command {nameof(SendPasswordResetMailCommand)} executed successfully for UserId: {user.Id}",
                    cancellationToken);
            },
            onError: async (mailCommand, exception) =>
            {
                await _logger.LogByExceptionSeverityAsync(
                    $"Error occurred during email verification: {mailCommand.VerifyToken}",
                    exception,
                    cancellationToken);
            },
            cancellationToken: cancellationToken
        );

        return result;
    }

    public async Task<Result<CommandResponse>> PasswordResetAsync(PasswordResetCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByPasswordResetTokenAsync(command.Token, cancellationToken);
        if (user is null)
            return Result<CommandResponse>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound);

        if (user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
            return Result<CommandResponse>.Fail(ErrorMessages.Expired.PasswordResetToken,
                ErrorCodes.PasswordResetTokenExpired);

        var result = await ExecuteCommandAsync(
            command: command,
            action: async () =>
            {
                user.ResetPassword(command.NewPassword);

                await _userRepository.UpdateAsync(user, cancellationToken);
                return Result<CommandResponse>.Ok(Success(user.UpdatedAt, correlationId: user.Id));
            },
            onFailure: async (_, res) =>
            {
                var errorMessage = string.Join(", ", res.GetErrors());
                await _logger.LogWarningAsync($"Command failed: {errorMessage}", cancellationToken);
            },
            onSuccess: async (_, _) =>
            {
                await _logger.LogInformationAsync(
                    $"Command {nameof(PasswordResetCommand)} executed successfully for UserId: {user.Id}",
                    cancellationToken);
            },
            onError: async (mailCommand, exception) =>
            {
                await _logger.LogByExceptionSeverityAsync(
                    $"Error occurred during email verification: {mailCommand.Token}",
                    exception,
                    cancellationToken);
            },
            cancellationToken: cancellationToken
        );

        return result;
    }

    public async Task<Result<CommandResponse>> SendPasswordResetMailAsync(SendPasswordResetMailCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(command.Email, cancellationToken);
        if (user is null)
            return Result<CommandResponse>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound);

        var result = await ExecuteCommandAsync(
            command: command,
            action: async () =>
            {
                user.CreatePasswordResetToken();
                await _userRepository.UpdateAsync(user, cancellationToken);

                var email = await _emailFactory.CreateAsync(
                    type: EmailEventType.PasswordReset,
                    to: user.Email,
                    metadata: new Dictionary<string, string?>
                    {
                        { "name", user.Name },
                        { "passwordResetToken", user.PasswordResetToken },
                        { "passwordResetTokenExpiresAt", user.PasswordResetTokenExpiresAt.ToString() }
                    }
                );

                await _rabbitMqEmailPublisher.PublishEmailEventAsync(email, RabbitMqQueues.EmailQueue,
                    cancellationToken);

                return Result<CommandResponse>.Ok(Success(user.UpdatedAt, correlationId: user.Id));
            },
            onFailure: async (_, res) =>
            {
                var errorMessage = string.Join(", ", res.GetErrors());
                await _logger.LogWarningAsync($"Command failed: {errorMessage}", cancellationToken);
            },
            onSuccess: async (_, _) =>
            {
                await _logger.LogInformationAsync(
                    $"Command {nameof(SendPasswordResetMailCommand)} executed successfully for UserId: {user.Id}",
                    cancellationToken);
            },
            onError: async (mailCommand, exception) =>
            {
                await _logger.LogByExceptionSeverityAsync(
                    $"Error occurred during password reset email: {mailCommand.Email}",
                    exception,
                    cancellationToken);
            },
            cancellationToken: cancellationToken
        );

        return result;
    }

    public async Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(command.Token, cancellationToken);
        if (refreshToken is null)
            return Result<TokenResponse>.Fail(ErrorMessages.NotFound.RefreshToken, ErrorCodes.RefreshTokenNotFound);

        if (refreshToken.IsUsed || refreshToken.IsRevoked)
            return Result<TokenResponse>.Fail(ErrorMessages.Invalid.RefreshToken, ErrorCodes.InvalidRefreshToken);

        if (refreshToken.ExpiresAt < DateTime.UtcNow)
            return Result<TokenResponse>.Fail(ErrorMessages.Expired.RefreshToken, ErrorCodes.RefreshTokenExpired);

        var user = await _userRepository.GetUserByIdAsync(refreshToken.UserId, cancellationToken);
        if (user is null)
            return Result<TokenResponse>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var newRefreshToken = await _refreshTokenService.CreateRefreshToken(
                userId: user.Id,
                ipAddress: command.IpAddress,
                cancellationToken: cancellationToken
            );

            await _refreshTokenService.MarkRefreshTokenAsUsedAsync(refreshToken, cancellationToken);

            var tokenResponse = await _tokenService.GenerateTokenAsync(
                userId: user.Id,
                username: user.Username,
                role: user.Role.GetRoleName(),
                refreshToken: newRefreshToken.Token,
                email: user.Email,
                refreshTokenExpires: newRefreshToken.ExpiresAt
            );

            await _logger.LogInformationAsync($"Refresh token was regenerated for :{user.Id}", cancellationToken);
            await SaveAndCommitAsync(cancellationToken);

            return Result<TokenResponse>.Ok(tokenResponse);
        }
        catch (Exception e)
        {
            await _unitOfWork.SafeRollbackAsync(cancellationToken);
            await _logger.LogByExceptionSeverityAsync("Error occurred during refresh token generation", e,
                cancellationToken);
            return Result<TokenResponse>.Fail("Unexpected error occurred during refresh token generation.");
        }
    }

    private async Task<Result<TokenResponse>> SendOtpAsync(Domain.Entity.User user, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            user.GenerateOtpCode();
            await _userRepository.UpdateAsync(user, cancellationToken);

            var email = await _emailFactory.CreateAsync(
                type: EmailEventType.Otp,
                to: user.Email,
                metadata: new Dictionary<string, string?>
                {
                    { "name", user.Name },
                    { "otpCode", user.OtpCode },
                    { "otpCodeExpiresAt", user.OtpCodeExpiresAt.ToString() }
                }
            );

            await _rabbitMqEmailPublisher.PublishEmailEventAsync(email, RabbitMqQueues.EmailQueue,
                cancellationToken);

            await SaveAndCommitAsync(cancellationToken);

            var tokenResponse = _tokenService.CreateTokenResponseFor2Fa(user.Id);

            await _logger.LogInformationAsync($"OTP sent successfully for UserId: {user.Id}", cancellationToken);
            return Result<TokenResponse>.Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            await _unitOfWork.SafeRollbackAsync(cancellationToken);
            await _logger.LogByExceptionSeverityAsync("Error occurred during OTP generation", ex, cancellationToken);
            return Result<TokenResponse>.Fail("Unexpected error occurred during OTP generation.");
        }
    }

    private async Task<Result<TokenResponse>> PerformLoginTransactionAsync(Domain.Entity.User user,
        bool isLoginSuccessful, string? ipAddress, string? userAgent, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await _loginHistoryService.AddLoginHistory(
                user.Id,
                ipAddress,
                userAgent,
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
                ipAddress,
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
            await _logger.LogByExceptionSeverityAsync("Error occurred during login", ex, cancellationToken);
            return Result<TokenResponse>.Fail("Unexpected error occurred during login.");
        }
    }
}