using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Surveillance.Function.Notification.Infrastructure
{
    internal class SendGridEmailService : IEmailService
    {
        private readonly string sendGridApiKey;

        public SendGridEmailService(IOptions<AppConfiguration> appConfig)
        {
            if (appConfig?.Value is null)
            {
                throw new ArgumentNullException(nameof(appConfig));
            }

            this.sendGridApiKey = appConfig.Value.SendGridApiKey;
        }

        public async Task SendEmailAsync(EmailModel email)
        {
            var emailMessage = BuildEmail(email);
            var client = new SendGridClient(this.sendGridApiKey);
            var response = await client.SendEmailAsync(emailMessage);
            var content = await response.Body.ReadAsStringAsync();

            if (!response.StatusCode.Equals(HttpStatusCode.Accepted))
            {
                throw new Exception($"Could not send email. HttpStatusCode: {response.StatusCode}, Details: {content}");
            }
        }

        private SendGridMessage BuildEmail(EmailModel emailData)
        {
            var senderEmail = BuildEmailAddress(emailData.SenderEmail, emailData.SenderName);
            var receiverEmail = BuildEmailAddress(emailData.ReceiverEmail, emailData.ReceiverName);
            var mailTextContent = emailData.Message;
            var emailMessage = MailHelper.CreateSingleEmail(
                senderEmail,
                receiverEmail,
                emailData.Subject,
                mailTextContent,
                null);
            emailMessage.TrackingSettings = CreateTrackingSettings();
            AddSubstitutionsToEmailMessage(emailMessage, emailData.Substitutions);

            return emailMessage;
        }

        private TrackingSettings CreateTrackingSettings()
        {
            return new TrackingSettings()
            {
                ClickTracking = new ClickTracking()
                {
                    Enable = false
                }
            };
        }

        private EmailAddress BuildEmailAddress(string emailAddress, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                return new EmailAddress(emailAddress);
            }

            return new EmailAddress(emailAddress, name);
        }

        private void AddSubstitutionsToEmailMessage(
            SendGridMessage emailMessage,
            IDictionary<string, string> substitutions)
        {
            if (substitutions != null)
            {
                var substitutionsDict = new Dictionary<string, string>(substitutions);
                emailMessage.AddSubstitutions(substitutionsDict);
            }
        }
    }
}
