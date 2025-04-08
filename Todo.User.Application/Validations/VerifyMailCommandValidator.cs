using FluentValidation;
using Todo.Shared.Contracts.Constant;
using Todo.User.Application.Command;

namespace Todo.User.Application.Validations;

public class VerifyMailCommandValidator : AbstractValidator<VerifyMailCommand>
{
    public VerifyMailCommandValidator()
    {
        RuleFor(command => command.VerifyToken)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.VerifyToken);
    }
}