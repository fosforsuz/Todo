using System;
using FluentValidation;
using Todo.Shared.Contracts.Constant;
using Todo.User.Application.Command;

namespace Todo.User.Application.Validations;

public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{

    public UpdatePasswordCommandValidator()
    {
        RuleFor(command => command.OldPassword)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.Password)
            .MinimumLength(6)
            .WithMessage(string.Format(ErrorMessages.MinLength.MinimumLength, 6))
            .MaximumLength(50)
            .WithMessage(string.Format(ErrorMessages.MaxLength.MaximumLength, 50));

        RuleFor(command => command.NewPassword)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.Password)
            .MinimumLength(6)
            .WithMessage(string.Format(ErrorMessages.MinLength.MinimumLength, 6))
            .MaximumLength(50)
            .WithMessage(string.Format(ErrorMessages.MaxLength.MaximumLength, 50));

        RuleFor(command => command.ConfirmPassword)
            .Equal(command => command.NewPassword)
            .WithMessage(ErrorMessages.Match.Password);

    }
}
