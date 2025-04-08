using MediatR;
using Todo.SharedKernel.Enums;
using Todo.SharedKernel.Results;

namespace Todo.User.Application.Query.Handler;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, Result<List<string>>>
{
    public Task<Result<List<string>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = Enum.GetNames(typeof(Role))
            .Select(role => role.ToLower())
            .ToList();

        return Task.FromResult(Result<List<string>>.Ok(roles));
    }
}