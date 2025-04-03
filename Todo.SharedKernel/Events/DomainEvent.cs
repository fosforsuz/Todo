namespace Todo.SharedKernel.Events;

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}