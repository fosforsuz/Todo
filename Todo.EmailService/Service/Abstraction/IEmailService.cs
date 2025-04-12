using Todo.SharedKernel.Events;

namespace Todo.EmailService.Service.Abstraction;

public interface IEmailService
{
    public Task SendEmailAsync(EmailEvent emailEvent, CancellationToken cancellationToken);
}