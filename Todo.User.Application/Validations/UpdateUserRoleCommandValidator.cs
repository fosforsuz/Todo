using System;
using FluentValidation;
using Todo.Shared.Contracts.Constant;
using Todo.User.Application.Command;

namespace Todo.User.Application.Validations;

public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
{
    public UpdateUserRoleCommandValidator()
    {

        RuleFor(command => command.UserId)
            .NotEmpty()
            .WithMessage(ErrorMessages.NotEmpty.UserId);

        RuleFor(command => command.Role)
            .IsInEnum()
            .WithMessage(ErrorMessages.Invalid.Role);
    }
}
