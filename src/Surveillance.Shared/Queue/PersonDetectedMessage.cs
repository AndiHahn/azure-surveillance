using System;

namespace Surveillance.Shared.Queue
{
    public class PersonDetectedMessage
    {
        public double Confidence { get; set; }

        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"{nameof(Confidence)}: {Confidence}, {nameof(Timestamp)}: {Timestamp}";
        }
    }
}
