using System;

namespace Todo.User.Application.Command.Abstraction;

public abstract class IdentifiableCommand
{
    public Guid UserId { get; protected set; }
    public void SetUserId(Guid userId) => UserId = userId;
}
