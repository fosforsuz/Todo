using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;

namespace Todo.User.Application.Command.Handlers;

public class SendVerifyMailCommandHandler : IRequestHandler<SendVerifyMailCommand, Result<CommandResponse>>
{
    private readonly IAuthService _authService;

    public SendVerifyMailCommandHandler(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    public async Task<Result<CommandResponse>> Handle(SendVerifyMailCommand request,
        CancellationToken cancellationToken)
    {
        return await _authService.SendVerifyMailAsync(request, cancellationToken);
    }
}