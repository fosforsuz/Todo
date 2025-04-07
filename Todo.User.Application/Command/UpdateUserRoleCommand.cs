using System;
using Todo.SharedKernel.Enums;
using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class UpdateUserRoleCommand : IdentifiableCommand
{
    public Role Role { get; set; }
}
