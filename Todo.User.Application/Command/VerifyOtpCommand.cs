using MediatR;
using Todo.SharedKernel.Results;
using Todo.User.Infrastructure.Models;

namespace Todo.User.Application.Command;

public class VerifyOtpCommand : IRequest<Result<TokenResponse>>
{
    public Guid UserId { get; set; }
    public string Otp { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
}