using System;
using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class UpdatePasswordCommand : IdentifiableCommand
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
