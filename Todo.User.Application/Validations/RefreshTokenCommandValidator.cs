using System;
using FluentValidation;
using Todo.Shared.Contracts.Constant;
using Todo.User.Application.Command;

namespace Todo.User.Application.Validations;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(command => command.Token)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.RefreshToken);
    }
}
