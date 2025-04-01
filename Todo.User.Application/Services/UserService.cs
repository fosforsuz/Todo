using Todo.Shared.Abstraction;
using Todo.User.Application.Abstraction;
using Todo.User.Infrastructure.Abstraction;

namespace Todo.User.Application.Services;

public class UserService : IUserService
{
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UserService(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = _unitOfWork.GetCustomRepository<IUserRepository>() ??
                          throw new ArgumentNullException(nameof(unitOfWork));
    }
}