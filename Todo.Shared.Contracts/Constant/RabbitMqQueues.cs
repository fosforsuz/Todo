namespace Todo.Shared.Contracts.Constant;

public static class RabbitMqQueues
{
    public const string LogEventQueue = "log_event_queue";
    public const string LogEventDlqQueue = "log_event_dlq_queue";
    public const string EmailQueue = "email_queue";
}