using System;
using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class LogoutCommand
{
    public required string RefreshToken { get; init; }
}