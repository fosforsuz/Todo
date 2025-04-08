using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;
using Todo.User.Application.Dto;

namespace Todo.User.Application.Query.Handler;

public class GetListUserQueryHandler : IRequestHandler<GetListUsersQuery, Result<PaginatedList<UserDto>>>
{
    private readonly IUserService _userService;

    public GetListUserQueryHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<PaginatedList<UserDto>>> Handle(GetListUsersQuery request,
        CancellationToken cancellationToken)
    {
        return await _userService.GetListUsersQuery(request, cancellationToken);
    }
}