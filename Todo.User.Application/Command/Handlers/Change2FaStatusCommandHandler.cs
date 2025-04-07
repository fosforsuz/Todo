using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;

namespace Todo.User.Application.Command.Handlers;

public class Change2FaStatusCommandHandler : IRequestHandler<Change2FaStatusCommand, Result<CommandResponse>>
{
    private readonly IAuthService _authService;

    public Change2FaStatusCommandHandler(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    public async Task<Result<CommandResponse>> Handle(Change2FaStatusCommand request,
        CancellationToken cancellationToken)
    {
        return await _authService.Change2FaStatusAsync(request, cancellationToken);
    }
}