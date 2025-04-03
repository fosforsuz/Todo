namespace Todo.SharedKernel.Response;

public class CommandResponse
{
    public CommandResponse(DateTime createdDate, string? location, Guid? correlationId)
    {
        ResponseId = Guid.NewGuid();
        CreatedDate = createdDate;
        Location = location;
        ResponseDate = DateTime.UtcNow;
        CorrelationId = correlationId;
    }

    public Guid ResponseId { get; protected set; }
    public Guid? CorrelationId { get; protected set; }
    public DateTime ResponseDate { get; protected set; }
    public DateTime CreatedDate { get; protected set; }
    public string? Location { get; protected set; }
}