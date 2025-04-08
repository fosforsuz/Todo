namespace Todo.Shared.Contracts.Config;

public class RabbitMqConfig
{
    public string HostName { get; set; } = null!;
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public bool AutomaticRecoveryEnabled { get; set; } = true;
    public string ConnectionName { get; set; } = "TodoService";
}