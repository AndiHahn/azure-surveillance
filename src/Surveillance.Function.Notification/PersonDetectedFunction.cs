using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Surveillance.Function.Notification.Infrastructure;
using Surveillance.Shared.Queue;

namespace Surveillance.Function.Notification
{
    internal class PersonDetectedFunction
    {
        private readonly IEmailService emailService;
        private readonly AppConfiguration appConfiguration;

        public PersonDetectedFunction(
            IEmailService emailService,
            IOptions<AppConfiguration> options)
        {
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.appConfiguration = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        [FunctionName("PersonDetectedFunction")]
        public async Task PersonDetectedRun(
            [QueueTrigger("person-detected-queue", Connection = "PersonDetectedQueueStorageConnectionString")] PersonDetectedMessage queueMessage,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {queueMessage}");

            await this.emailService.SendEmailAsync(new EmailModel
            {
                SenderName = this.appConfiguration.SenderName,
                SenderEmail = this.appConfiguration.SenderEmail,
                ReceiverName = this.appConfiguration.ReceiverName,
                ReceiverEmail = this.appConfiguration.ReceiverEmail,
                Subject = "Person detected",
                Message = "Hello $firstname$,\r\n\r\nA person was detected at: $timestamp$ with a probability of $probability$.\r\n\r\nBest regards,\r\nYour surveillance system",
                Substitutions = new Dictionary<string, string>
                {
                    { "$firstname$", this.appConfiguration.ReceiverName.Split(" ").FirstOrDefault() },
                    { "$timestamp$", queueMessage.Timestamp.ToString("dd.MM.yyyy HH:mm:ss") },
                    { "$probability$", (queueMessage.Confidence * 100).ToString() }
                }
            });

            log.LogInformation("Successfully sent email.");
        }
    }
}
