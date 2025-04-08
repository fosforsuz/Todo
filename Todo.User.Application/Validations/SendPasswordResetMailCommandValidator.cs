using FluentValidation;
using Todo.Shared.Contracts.Constant;
using Todo.User.Application.Command;

namespace Todo.User.Application.Validations;

public class SendPasswordResetMailCommandValidator : AbstractValidator<SendPasswordResetMailCommand>
{
    public SendPasswordResetMailCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.Email)
            .EmailAddress()
            .WithMessage(ErrorMessages.Invalid.Email);
    }
}