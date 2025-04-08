namespace Todo.Shared.Contracts.Config;

public class ElasticConfig
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? FingerPrint { get; set; }
}