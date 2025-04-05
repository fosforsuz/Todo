using Microsoft.AspNetCore.Hosting;
using Todo.SharedKernel.Enums;
using Todo.SharedKernel.Events;
using Todo.SharedKernel.Extensions;
using Todo.SharedKernel.Factory;

namespace Todo.User.Infrastructure.Factory;

public class EmailFactory : IEmailFactory
{
    private readonly IWebHostEnvironment _env;

    public EmailFactory(IWebHostEnvironment env)
    {
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    public async Task<EmailEvent> CreateAsync(EmailEventType type, string to, Dictionary<string, string?> metadata)
    {
        var templatePath = Path.Combine(_env.WebRootPath, "templates", $"{type}.html");

        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Email template not found: {templatePath}");

        var html = await File.ReadAllTextAsync(templatePath);

        foreach (var kvp in metadata)
        {
            html = html.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
        }

        return new EmailEvent
        {
            To = to,
            Subject = type.GetEmailEventTypeSubject(),
            HtmlBody = html,
            Type = type,
            Metadata = metadata
        };
    }
}