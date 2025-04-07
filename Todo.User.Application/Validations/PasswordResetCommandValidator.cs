using System;
using FluentValidation;
using Todo.Shared.Contracts.Constant;
using Todo.User.Application.Command;

namespace Todo.User.Application.Validations;

public class PasswordResetCommandValidator : AbstractValidator<PasswordResetCommand>
{
    public PasswordResetCommandValidator()
    {
        RuleFor(command => command.Token)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.PasswordResetToken);

        RuleFor(command => command.NewPassword)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.NewPassword)
            .MinimumLength(6)
            .WithMessage(ErrorMessages.MinLength.NewPassword);

        RuleFor(command => command.ConfirmPassword)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.Password)
            .Equal(command => command.NewPassword)
            .WithMessage(ErrorMessages.Match.Password);
    }
}
