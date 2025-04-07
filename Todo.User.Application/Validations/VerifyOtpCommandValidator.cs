using FluentValidation;
using Todo.Shared.Contracts.Constant;
using Todo.User.Application.Command;

namespace Todo.User.Application.Validations;

public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.UserId);

        RuleFor(command => command.Otp)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.Otp);
    }
}