using MediatR;
using Todo.SharedKernel.Results;
using Todo.User.Infrastructure.Models;

namespace Todo.User.Application.Command;

public class LoginCommand : IRequest<Result<TokenResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}