using FluentValidation;
using Todo.Shared.Contracts.Constant;
using Todo.User.Application.Command;

namespace Todo.User.Application.Validations;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage(ErrorMessages.NotEmpty.UserId);
    }
}