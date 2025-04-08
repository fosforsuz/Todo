using MediatR;
using Todo.SharedKernel.Results;
using Todo.User.Application.Dto;

namespace Todo.User.Application.Query;

public class GetUserByUsernameQuery : IRequest<Result<UserDto>>
{
    public string Username { get; set; } = string.Empty;
}