using Azure.Storage.Queues;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Surveillance.ConsoleClient
{
    internal class Program
    {
        private const int nrOfMessagesWithoutPerson = 5000;
        private const int nrOfMessagesWithPerson = 0;

        private static readonly string[] availableImagesWithPerson = new[]
        {
            "https://survimgstorage.blob.core.windows.net/camera/FotoPers1.PNG",
            "https://survimgstorage.blob.core.windows.net/camera/FotoPers3.PNG",
            "https://survimgstorage.blob.core.windows.net/camera/FotoPers4.PNG",
            "https://survimgstorage.blob.core.windows.net/camera/FotoPers5.PNG",
            "https://survimgstorage.blob.core.windows.net/camera/FotoPers6.PNG",
        };

        private static readonly string[] availableImagesWithoutPerson = new[]
        {
            "https://survimgstorage.blob.core.windows.net/camera/FotoNoPers.PNG",
            "https://survimgstorage.blob.core.windows.net/camera/FotoPers2.PNG",
        };

        private static QueueClient queueClient;

        static Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string queueName = config.GetSection("QueueName").Value;
            string storageConnectionString = config.GetSection("QueueStorageConnectionString").Value;

            queueClient = new QueueClient(storageConnectionString, queueName);

            var tasks1 = Enumerable.Range(0, nrOfMessagesWithPerson).Select(_ => SendQueueMessageAsync(availableImagesWithPerson.First()));
            var tasks2 = Enumerable.Range(0, nrOfMessagesWithoutPerson).Select(_ => SendQueueMessageAsync(availableImagesWithoutPerson.First()));

            return Task.WhenAll(tasks1.Concat(tasks2));
        }

        private static Task SendQueueMessageAsync(string imageUrl)
        {
            var message = new EventGridEvent
            {
                Id = Guid.NewGuid().ToString(),
                EventType = "Microsoft.Storage.BlobCreated",
                EventTime = DateTime.UtcNow,
                Data = new BlobCreatedData
                {
                    ContentType = "image/png",
                    Url = imageUrl,
                }
            };

            var messageJson = JsonConvert.SerializeObject(message);

            return queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(messageJson)));
        }
    }
}
