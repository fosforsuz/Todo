using Todo.SharedKernel.Infrastructure;
using Todo.User.Infrastructure.Abstraction;
using Todo.User.Infrastructure.Data;

namespace Todo.User.Infrastructure.Persistence;

internal class UserRepository : Repository<Domain.Entity.User>, IUserRepository
{
    public UserRepository(UserDbContext context) : base(context)
    {
    }

    public async Task<Domain.Entity.User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await GetSingleAsync(
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

    public async Task<Domain.Entity.User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await GetSingleAsync(
            predicate: user => user.EmailLower == normalizedEmail && user.IsActive,
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

    public async Task<Domain.Entity.User?> GetUserByEmailVerificationTokenAsync(
        string emailVerificationToken,
        CancellationToken cancellationToken)
    {
        return await GetSingleAsync(
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

    public async Task<Domain.Entity.User?> GetUserByPasswordResetTokenAsync(string passwordResetToken, CancellationToken cancellationToken)
    {
        return await GetSingleAsync(
            predicate: user => user.PasswordResetToken == passwordResetToken && user.IsActive,
            selector: user => new Domain.Entity.User
            {
                Name = user.Name,
                Username = user.Username,
                UsernameLower = user.UsernameLower,
                Email = user.Email,
                EmailLower = user.EmailLower,
                HashedPassword = string.Empty,
                PasswordResetToken = user.PasswordResetToken,
                PasswordResetTokenExpiresAt = user.PasswordResetTokenExpiresAt
            },
            cancellationToken: cancellationToken
        );
    }

    public async Task<Domain.Entity.User?> GetUserForLoginAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await GetSingleAsync(
            user => normalizedEmail.Equals(user.EmailLower) && user.IsActive,
            user => new Domain.Entity.User
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Username = user.Username,
                UsernameLower = user.UsernameLower,
                EmailLower = user.EmailLower,
                HashedPassword = user.HashedPassword,
                Is2FaEnabled = user.Is2FaEnabled,
                IsEmailVerified = user.IsEmailVerified
            },
            cancellationToken: cancellationToken
        );
    }
}