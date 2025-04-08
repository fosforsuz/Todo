using System.Linq.Expressions;
using Todo.Shared.Contracts.Constant;
using Todo.SharedKernel.Abstraction;
using Todo.SharedKernel.Extensions;
using Todo.SharedKernel.Logger;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;
using Todo.User.Application.Command;
using Todo.User.Application.Dto;
using Todo.User.Application.Query;
using Todo.User.Infrastructure.Abstraction;

namespace Todo.User.Application.Services;

public class UserService : BaseService<UserService>, IUserService
{
    private readonly ILoggerService<UserService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UserService(IUnitOfWork unitOfWork, ILoggerService<UserService> logger) : base(unitOfWork, logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userRepository = _unitOfWork.GetCustomRepository<IUserRepository>() ??
                          throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<UserDto>> GetUserById(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetSingleAsync(
            user => user.Id == query.UserId && user.IsActive,
            user => new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.GetRoleName(),
                Is2FaEnabled = user.Is2FaEnabled,
                IsEmailVerified = user.IsEmailVerified,
                IsNotificationEnabled = user.NotificationEnabled,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            },
            cancellationToken: cancellationToken
        );

        return user is null
            ? Result<UserDto>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound)
            : Result<UserDto>.Ok(user);
    }

    public async Task<Result<UserDto>> GetUserByUsername(GetUserByUsernameQuery query,
        CancellationToken cancellationToken)
    {
        var username = query.Username.ToLower();
        var user = await _userRepository.GetSingleAsync(
            user => user.UsernameLower == username,
            user => new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.GetRoleName(),
                Is2FaEnabled = user.Is2FaEnabled,
                IsEmailVerified = user.IsEmailVerified,
                IsNotificationEnabled = user.NotificationEnabled,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            },
            cancellationToken: cancellationToken
        );

        return user is null
            ? Result<UserDto>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound)
            : Result<UserDto>.Ok(user);
    }

    public async Task<Result<PaginatedList<UserDto>>> GetListUsersQuery(GetListUsersQuery query,
        CancellationToken cancellationToken)
    {
        Expression<Func<Domain.Entity.User, bool>> predicate = user => user.IsActive;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            predicate = predicate.AndAlso(user =>
                user.UsernameLower.Contains(search) ||
                user.Name.ToLower().Contains(search) ||
                user.EmailLower.Contains(search) ||
                (user.Phone != null && user.Phone.Contains(search)));
        }

        var skip = (query.Page - 1) * query.PageSize;

        var users = await _userRepository.GetAsync(
            predicate,
            user => new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.GetRoleName(),
                Is2FaEnabled = user.Is2FaEnabled,
                IsEmailVerified = user.IsEmailVerified,
                IsNotificationEnabled = user.NotificationEnabled,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            },
            skip,
            query.PageSize,
            query.SortBy,
            query.IsDescending,
            cancellationToken
        );

        var totalCount = await _userRepository.CountAsync(predicate, cancellationToken);

        var paginated = new PaginatedList<UserDto>
        {
            Items = users,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };

        return Result<PaginatedList<UserDto>>.Ok(paginated);
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

            var response = Success(user.CreatedAt, null, user.Id);
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

    public async Task<Result<CommandResponse>> UpdateUserAsync(UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result<CommandResponse>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound);

        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            var phoneValidationResult = await ValidatePhoneUniquenessAsync(command.Phone, user.Id,
                cancellationToken);
            if (phoneValidationResult.HasError)
                return Result<CommandResponse>.Fail(
                    ErrorMessages.Exist.PhoneAlreadyExists,
                    ErrorCodes.PhoneAlreadyExists);
        }

        var result = await ExecuteCommandAsync(
            command,
            async () =>
            {
                user.Update(
                    command.Name,
                    command.Phone,
                    command.IsNotificationEnabled
                );

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _logger.LogInformationAsync($"User updated successfully {user.Id}", cancellationToken);

                return Result<CommandResponse>.Ok(Success(user.UpdatedAt, null, user.Id));
            },
            onSuccess: async (_, _) =>
            {
                await _logger.LogInformationAsync($"User updated successfully {user.Id}", cancellationToken);
            },
            onError: async (_, ex) =>
            {
                await _logger.LogByExceptionSeverityAsync("An error occurred while updating user", ex,
                    cancellationToken);
            },
            onFailure: async (_, res) =>
            {
                var errorMessages = string.Join(", ", res.GetErrors());
                await _logger.LogWarningAsync($"An error occurred while updating user. {errorMessages}",
                    cancellationToken);
            },
            cancellationToken: cancellationToken
        );

        return result;
    }

    public async Task<Result<CommandResponse>> UpdatePasswordAsync(UpdatePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result<CommandResponse>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound);

        if (!user.VerifyPassword(command.OldPassword))
            return Result<CommandResponse>.Fail(ErrorMessages.Invalid.Password, ErrorCodes.InvalidPassword);


        var result = await ExecuteCommandAsync(
            command,
            async () =>
            {
                user.UpdatePassword(command.NewPassword);
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _logger.LogInformationAsync($"User password updated successfully {user.Id}", cancellationToken);

                return Result<CommandResponse>.Ok(Success(user.CreatedAt, null, user.Id));
            },
            onSuccess: async (_, _) =>
            {
                await _logger.LogInformationAsync($"User password updated successfully {user.Id}", cancellationToken);
            },
            onError: async (_, ex) =>
            {
                await _logger.LogByExceptionSeverityAsync("An error occurred while updating user password", ex,
                    cancellationToken);
            },
            onFailure: async (_, res) =>
            {
                var errorMessages = string.Join(", ", res.GetErrors());
                await _logger.LogWarningAsync($"An error occurred while updating user password. {errorMessages}",
                    cancellationToken);
            },
            cancellationToken: cancellationToken
        );

        return result;
    }

    public async Task<Result<CommandResponse>> UpdateUserRoleAsync(UpdateUserRoleCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result<CommandResponse>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound);

        var result = await ExecuteCommandAsync(
            command,
            async () =>
            {
                user.UpdateRole(command.Role);
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _logger.LogInformationAsync($"User role updated successfully {user.Id}", cancellationToken);

                return Result<CommandResponse>.Ok(Success(user.UpdatedAt, null, user.Id));
            },
            onSuccess: async (_, _) =>
            {
                await _logger.LogInformationAsync($"User role updated successfully {user.Id}", cancellationToken);
            },
            onError: async (_, ex) =>
            {
                await _logger.LogByExceptionSeverityAsync("An error occurred while updating user role", ex,
                    cancellationToken);
            },
            onFailure: async (_, res) =>
            {
                var errorMessages = string.Join(", ", res.GetErrors());
                await _logger.LogWarningAsync($"An error occurred while updating user role. {errorMessages}",
                    cancellationToken);
            },
            cancellationToken: cancellationToken
        );

        return result;
    }

    public async Task<Result<CommandResponse>> DeleteUserAsync(DeleteUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result<CommandResponse>.Fail(ErrorMessages.NotFound.User, ErrorCodes.UserNotFound);

        var result = await ExecuteCommandAsync(
            command,
            async () =>
            {
                user.Delete();

                await _userRepository.UpdateAsync(user, cancellationToken);

                await _logger.LogInformationAsync($"User deleted successfully {user.Id}", cancellationToken);

                return Result<CommandResponse>.Ok(Success(user.UpdatedAt, null, user.Id));
            },
            onSuccess: async (_, _) =>
            {
                await _logger.LogInformationAsync($"User deleted successfully {user.Id}", cancellationToken);
            },
            onError: async (_, ex) =>
            {
                await _logger.LogByExceptionSeverityAsync("An error occurred while deleting user", ex,
                    cancellationToken);
            },
            onFailure: async (_, res) =>
            {
                var errorMessages = string.Join(", ", res.GetErrors());
                await _logger.LogWarningAsync($"An error occurred while deleting user. {errorMessages}",
                    cancellationToken);
            },
            cancellationToken: cancellationToken
        );

        return result;
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

        if (string.IsNullOrEmpty(phone))
            return result;

        var phoneValidationResult = await ValidatePhoneUniquenessAsync(phone, userId, cancellationToken);
        if (phoneValidationResult.HasError)
            result.AddError(phoneValidationResult.GetErrorCodes()[0], phoneValidationResult.GetErrors()[0]);

        return result;
    }

    private async Task<Result> ValidatePhoneUniquenessAsync(string phone, Guid? userId,
        CancellationToken cancellationToken)
    {
        var result = new Result();

        if (!string.IsNullOrWhiteSpace(phone) &&
            await _userRepository.AnyAsync(x => x.Phone == phone && (!userId.HasValue || x.Id != userId.Value),
                cancellationToken))
            result.AddError(ErrorMessages.Exist.PhoneAlreadyExists, ErrorCodes.PhoneAlreadyExists);

        return result;
    }
}