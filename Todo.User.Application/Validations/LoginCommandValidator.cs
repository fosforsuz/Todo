using System;
using FluentValidation;
using Todo.Shared.Contracts.Constant;
using Todo.User.Application.Command;

namespace Todo.User.Application.Validations;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{

    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage(ErrorMessages.NotEmpty.Email);

        RuleFor(command => command.Password)
            .NotEmpty().WithMessage(ErrorMessages.NotEmpty.Password);
    }
}
