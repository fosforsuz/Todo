using System;
using FluentValidation;
using Todo.Shared.Contracts.Constant;
using Todo.User.Application.Command;

namespace Todo.User.Application.Validations;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.Name)
            .MinimumLength(3)
            .WithMessage(string.Format(ErrorMessages.MinLength.MinimumLength, 3))
            .MaximumLength(100)
            .WithMessage(string.Format(ErrorMessages.MaxLength.MaximumLength, 100));

        RuleFor(command => command.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage(ErrorMessages.Invalid.Phone)
            .MaximumLength(20)
            .WithMessage(string.Format(ErrorMessages.MaxLength.MaximumLength, 20));
    }
}
