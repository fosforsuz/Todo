using MediatR;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;
using Todo.User.Application.Dto;

namespace Todo.User.Application.Query.Handler;

public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, Result<UserDto>>
{
    private readonly IUserService _userService;

    public GetUserByUsernameQueryHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<UserDto>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserByUsername(request, cancellationToken);
    }
}