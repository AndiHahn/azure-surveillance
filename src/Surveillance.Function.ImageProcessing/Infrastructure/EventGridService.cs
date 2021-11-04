using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Options;
using Surveillance.Function.ImageProcessing.Infrastructure.Interface;

namespace Surveillance.Function.ImageProcessing.Infrastructure
{
    internal class EventGridService : IEventGridService
    {
        private readonly AppConfiguration appConfiguration;
        private readonly Lazy<EventGridClient> eventGridClient;

        public EventGridService(IOptions<AppConfiguration> options)
        {
            this.appConfiguration = options?.Value ?? throw new ArgumentNullException(nameof(options));

            var credentials = new TopicCredentials(options.Value.EventGridTopicKey);
            this.eventGridClient = new Lazy<EventGridClient>(CreateEventGridClient);
        }

        public Task PublishEventAsync(EventGridEvent @event)
        {
            var events = new List<EventGridEvent>();
            events.Add(@event);

            string hostName = new Uri(this.appConfiguration.EventGridTopicEndpoint).Host;
            return this.eventGridClient.Value.PublishEventsAsync(hostName, events);
        }

        private EventGridClient CreateEventGridClient()
        {
            var credentials = new TopicCredentials(this.appConfiguration.EventGridTopicKey);
            return new EventGridClient(credentials);
        }
    }
}
