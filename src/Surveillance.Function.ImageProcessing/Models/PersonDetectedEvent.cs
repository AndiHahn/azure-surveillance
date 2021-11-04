using System;
using Microsoft.Azure.EventGrid.Models;

#nullable enable

namespace Surveillance.Function.ImageProcessing.Models
{
    internal class PersonDetectedEvent : EventGridEvent
    {
        public PersonDetectedEvent(
            double confidence,
            DateTime? eventTime)
        {
            this.Id = Guid.NewGuid().ToString();
            this.EventType = "PersonDetected";
            this.Subject = "Person detection";
            this.EventTime = eventTime ?? DateTime.UtcNow;
            this.Data = new PersonDetectedEventData(confidence);
            this.DataVersion = "1.0";
        }
    }

    internal class PersonDetectedEventData
    {
        public PersonDetectedEventData(double confidence)
        {
            Confidence = confidence;
        }

        public double Confidence { get; }
    }
}
