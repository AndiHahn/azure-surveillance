using System.Threading.Tasks;

namespace Surveillance.Function.Notification.Infrastructure
{
    internal interface IEmailService
    {
        Task SendEmailAsync(EmailModel email);
    }
}
