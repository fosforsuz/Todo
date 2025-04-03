namespace Todo.Shared.Contracts.Config;

public class LogRabbitMqConfig
{
    public required string HostName { get; set; }
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
}