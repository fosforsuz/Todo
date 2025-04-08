using MediatR;
using Todo.SharedKernel.Results;

namespace Todo.User.Application.Query;

public class GetUserRolesQuery : IRequest<Result<List<string>>>;