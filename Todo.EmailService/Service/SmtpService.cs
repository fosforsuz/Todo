using Todo.EmailService.Service.Abstraction;
using Todo.SharedKernel.Events;

namespace Todo.EmailService.Service;

public class SmtpService : IEmailService
{
    public Task SendEmailAsync(EmailEvent emailEvent, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}