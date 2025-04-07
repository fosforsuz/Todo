using System;
using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class LogoutCommand
{
    public string RefreshToken { get; init; } = string.Empty;
}