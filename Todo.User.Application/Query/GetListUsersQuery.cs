using MediatR;
using Todo.SharedKernel.Request;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Dto;

namespace Todo.User.Application.Query;

public class GetListUsersQuery : PaginatedQuery, IRequest<Result<PaginatedList<UserDto>>>;