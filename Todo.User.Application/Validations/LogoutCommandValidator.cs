using System;
using FluentValidation;
using Todo.Shared.Contracts.Constant;
using Todo.User.Application.Command;

namespace Todo.User.Application.Validations;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{

    public LogoutCommandValidator()
    {
        RuleFor(command => command.RefreshToken).NotEmpty().WithMessage(ErrorMessages.NotEmpty.RefreshToken);
    }
}
