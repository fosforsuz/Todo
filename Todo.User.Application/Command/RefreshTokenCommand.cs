using MediatR;
using Todo.SharedKernel.Results;
using Todo.User.Infrastructure.Models;

namespace Todo.User.Application.Command;

public class RefreshTokenCommand : IRequest<Result<TokenResponse>>
{
    public string Token { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}