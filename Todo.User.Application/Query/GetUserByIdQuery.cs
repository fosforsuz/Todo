using MediatR;
using Todo.SharedKernel.Results;
using Todo.User.Application.Command.Abstraction;
using Todo.User.Application.Dto;

namespace Todo.User.Application.Query;

public class GetUserByIdQuery : IdentifiableCommand, IRequest<Result<UserDto>>;