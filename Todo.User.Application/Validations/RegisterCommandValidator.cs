using System;
using FluentValidation;
using Todo.Shared.Contracts.Constant;
using Todo.User.Application.Command;

namespace Todo.User.Application.Validations;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.Name)
            .MinimumLength(3)
            .WithMessage(string.Format(ErrorMessages.MinLength.MinimumLength, 3))
            .MaximumLength(100)
            .WithMessage(string.Format(ErrorMessages.MaxLength.MaximumLength, 100));

        RuleFor(command => command.Username)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.Username)
            .MinimumLength(3)
            .WithMessage(string.Format(ErrorMessages.MinLength.MinimumLength, 3))
            .MaximumLength(100)
            .WithMessage(string.Format(ErrorMessages.MaxLength.MaximumLength, 100));

        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.Email)
            .EmailAddress()
            .WithMessage(ErrorMessages.Invalid.Email)
            .MaximumLength(100)
            .WithMessage(string.Format(ErrorMessages.MaxLength.MaximumLength, 100));

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.Password)
            .MinimumLength(6)
            .WithMessage(string.Format(ErrorMessages.MinLength.MinimumLength, 6))
            .MaximumLength(50)
            .WithMessage(string.Format(ErrorMessages.MaxLength.MaximumLength, 50));

        RuleFor(command => command.PasswordConfirmation)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.PasswordConfirmation)
            .Equal(command => command.Password)
            .WithMessage(ErrorMessages.Match.Password);

        RuleFor(command => command.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage(ErrorMessages.Invalid.Phone)
            .MaximumLength(20)
            .WithMessage(string.Format(ErrorMessages.MaxLength.MaximumLength, 20));
    }
}
