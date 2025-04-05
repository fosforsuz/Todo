using Todo.Shared.Contracts.Constant;
using Todo.SharedKernel.Abstraction;
using Todo.SharedKernel.Extensions;
using Todo.SharedKernel.Logger;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;
using Todo.User.Application.Command;
using Todo.User.Infrastructure.Abstraction;

namespace Todo.User.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ILoggerService<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, ILoggerService<UserService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userRepository = _unitOfWork.GetCustomRepository<IUserRepository>() ??
                          throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<CommandResponse>> RegisterUserAsync(RegisterCommand registerCommand,
        CancellationToken cancellationToken)
    {
        var validation = await ValidateUserUniquenessAsync(registerCommand.Email, registerCommand.Username,
            registerCommand.Phone, null, cancellationToken);

        if (validation.HasError)
            return Result<CommandResponse>.Fail(
                "Some fields are invalid",
                validation.GetErrorCodes(),
                validation.GetErrors());

        var user = Domain.Entity.User.Create(
            registerCommand.Name,
            registerCommand.Username,
            registerCommand.Email,
            registerCommand.Phone,
            registerCommand.Password,
            registerCommand.Role.GetRoleNameFromString(),
            registerCommand.UtcOffset
        );


        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _logger.LogInformationAsync($"User registered successfully {user.Id}", cancellationToken);

            var response = CreateSuccessResponse(user);
            return Result<CommandResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            if (_unitOfWork.IsTransactionStarted)
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);


            await _logger.LogCriticalAsync("An error occurred while registering user", ex, cancellationToken);
            return Result<CommandResponse>.Fail("An error occurred while registering user");
        }
    }

    private async Task<Result> ValidateUserUniquenessAsync(string email, string username, string? phone, Guid? userId,
        CancellationToken cancellationToken)
    {
        var result = new Result();

        if (await _userRepository.AnyAsync(x => x.EmailLower == email && (!userId.HasValue || x.Id != userId.Value),
                cancellationToken))
            result.AddError(ErrorMessages.Exist.EmailAlreadyExists, ErrorCodes.EmailAlreadyExists);

        if (await _userRepository.AnyAsync(
                x => x.UsernameLower == username && (!userId.HasValue || x.Id != userId.Value), cancellationToken))
            result.AddError(ErrorMessages.Exist.UsernameAlreadyExists, ErrorCodes.UsernameAlreadyExists);

        if (!string.IsNullOrWhiteSpace(phone) &&
            await _userRepository.AnyAsync(x => x.Phone == phone && (!userId.HasValue || x.Id != userId.Value),
                cancellationToken))
            result.AddError(ErrorMessages.Exist.PhoneAlreadyExists, ErrorCodes.PhoneAlreadyExists);

        return result;
    }

    private static CommandResponse CreateSuccessResponse(Domain.Entity.User user)
    {
        return new CommandResponse(user.CreatedAt, string.Empty, user.Id);
    }
}